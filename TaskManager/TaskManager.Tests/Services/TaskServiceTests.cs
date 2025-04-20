using Xunit;
using Moq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TaskManager.API.Controllers;
using TaskManager.Domain.Entities;
using TaskManager.Infrastructure;
using TaskManager.Database;
using TaskManager.Models.Enums;
using TaskManagerAPI.Utils;
using TaskManager.Domain.DTOs;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TaskManager.Tests.Domain.Responses;
using TaskManager.Domain.Exceptions;
using TaskManager.API.Services.Interfaces;
using TaskManager.Application.Services;

namespace TaskManager.Tests.Services
{
    public class TaskServiceTests
    {
        private readonly TaskService _service;
        private readonly TaskManagerContext _context;
        private readonly Mock<TaskItemRepository> _taskRepoMock;
        private readonly Mock<TaskItemHistoricRepository> _taskHistoricRepoMock;
        private readonly Mock<TaskCommentRepository> _taskCommentRepoMock;
        private readonly Mock<ProjectRepository> _projectRepoMock;
        private readonly Mock<UserRepository> _userRepoMock;
        private readonly Mock<ConfigHelper> _configHelperMock;

        public TaskServiceTests()
        {
            var options = new DbContextOptionsBuilder<TaskManagerContext>()
                .UseInMemoryDatabase("TestDatabase")
                .Options;

            _context = new TaskManagerContext(options);

            _taskRepoMock = new Mock<TaskItemRepository>(_context);
            _taskHistoricRepoMock = new Mock<TaskItemHistoricRepository>(_context);
            _taskCommentRepoMock = new Mock<TaskCommentRepository>(_context);
            _projectRepoMock = new Mock<ProjectRepository>(_context);
            _userRepoMock = new Mock<UserRepository>(_context);
            _configHelperMock = new Mock<ConfigHelper>();
            _configHelperMock.Setup(x => x.NumberOfDaysToReport).Returns(30);

            _service = new TaskService(
                _taskRepoMock.Object,
                _projectRepoMock.Object,
                _taskHistoricRepoMock.Object,
                _taskCommentRepoMock.Object,
                _userRepoMock.Object,
                _configHelperMock.Object
            );
        }

        [Fact]
        public async Task GetAllTasksByProject_WhenTasksExist_ReturnsOk()
        {
            // Arrange
            long projectId = 1;
            List<TaskItem> taskItems = new List<TaskItem>
            {
                new TaskItem
                {
                    Id = 1,
                    Title = "Task 1",
                    Project = new Project("Project Title", "Project Description", DateTime.Now.AddDays(5), new User { Id = 1, Type = UserType.Manager }),
                    DueDate = DateTime.Now.AddDays(5),
                    TaskItemPriority = TaskItemPriority.Medium,
                    Status = TaskItemStatus.InProgress
                },
                new TaskItem
                {
                    Id = 2,
                    Title = "Task 2",
                    Project = new Project("Project Title 2", "Project Description 2", DateTime.Now.AddDays(5), new User { Id = 2, Type = UserType.Manager }),
                    DueDate = DateTime.Now.AddDays(5),
                    TaskItemPriority = TaskItemPriority.Medium,
                    Status = TaskItemStatus.InProgress
                }
            };

            _taskRepoMock.Setup(repo => repo.GetAllTasksByProjectId(projectId)).ReturnsAsync(taskItems);

            // Act
            TaskResponseDTO result = await _service.GetAllTasksByProject(projectId);

            // Assert
            string jsonResultValue = JsonConvert.SerializeObject(result);
            TasksResponse response = JsonConvert.DeserializeObject<TasksResponse>(jsonResultValue);

            Assert.True(response.Success);
            Assert.Equal("Tarefas recuperadas com sucesso.", response.Message.ToString());
            Assert.Equal(2, response.TaskItems.Count);
        }

        [Fact]
        public async Task GetAllTasksByProject_WhenNoTasksFound_ReturnsBadRequest()
        {
            // Arrange
            long projectId = 1;
            List<TaskItem> taskItems = new List<TaskItem>();

            _taskRepoMock.Setup(repo => repo.GetAllTasksByProjectId(It.IsAny<long>())).ThrowsAsync(new TmException("Nenhuma tarefa encontrada.", HttpStatusCode.BadRequest));

            // Act
            TmException result = await Assert.ThrowsAsync<TmException>(() => _service.GetAllTasksByProject(projectId));

            // Assert
            string jsonResultValue = JsonConvert.SerializeObject(result);
            TasksResponse response = JsonConvert.DeserializeObject<TasksResponse>(jsonResultValue);

            Assert.Equal((int)HttpStatusCode.BadRequest, (int)result.StatusCode);
            Assert.Equal("Nenhuma tarefa encontrada.", response.Message);
        }

        [Fact]
        public async Task GetAllTasksByProject_WhenTasksAreFiltered_ReturnsFilteredTasks()
        {
            // Arrange
            long projectId = 1;
            List<TaskItem> taskItems = new List<TaskItem>
            {
                new TaskItem
                {
                    Id = 1,
                    Title = "Task 1",
                    Project = new Project("Project Title", "Project Description", DateTime.Now.AddDays(5), new User { Id = 1, Type = UserType.Manager }),
                    DueDate = DateTime.Now.AddDays(5),
                    TaskItemPriority = TaskItemPriority.Medium,
                    Status = TaskItemStatus.Completed
                },
                new TaskItem
                {
                    Id = 2,
                    Title = "Task 2",
                    Project = new Project("Project Title", "Project Description", DateTime.Now.AddDays(5), new User { Id = 1, Type = UserType.Manager }),
                    DueDate = DateTime.Now.AddDays(5),
                    TaskItemPriority = TaskItemPriority.Medium,
                    Status = TaskItemStatus.InProgress
                }
            };

            _taskRepoMock.Setup(repo => repo.GetAllTasksByProjectId(projectId)).ReturnsAsync(taskItems);

            // Act
            TaskResponseDTO result = await _service.GetAllTasksByProject(projectId);

            // Assert
            string jsonResultValue = JsonConvert.SerializeObject(result);
            TasksResponse response = JsonConvert.DeserializeObject<TasksResponse>(jsonResultValue);

            Assert.True(response.Success);
            Assert.Equal("Tarefas recuperadas com sucesso.", response.Message.ToString());
            Assert.Equal(2, response.TaskItems.Count);
        }

        [Fact]
        public async Task CreateTask_WhenProjectExistsAndMaxTasksNotReached_ReturnsOk()
        {
            // Arrange
            CreateTaskDTO createTaskDTO = new CreateTaskDTO
            {
                ProjectId = 1,
                Title = "New Task",
                Description = "Description of the new task",
                DueDate = DateTime.Now.AddDays(5).ToString("yyyy-MM-dd"),
                TaskItemPriority = TaskItemPriority.Medium
            };

            Project project = new Project("Project 1", "Description of the project", DateTime.Now.AddDays(10), new User { Id = 1, Type = UserType.Manager }) { Id = 1 };
            _projectRepoMock.Setup(repo => repo.GetProjectById(It.IsAny<long>())).ReturnsAsync(project);
            _taskRepoMock.Setup(repo => repo.CountTaskItemsByProjectId(It.IsAny<long>())).ReturnsAsync(0);

            // Act
            TaskResponseDTO result = await _service.CreateTask(createTaskDTO);

            // Assert
            string jsonResultValue = JsonConvert.SerializeObject(result);
            TasksResponse response = JsonConvert.DeserializeObject<TasksResponse>(jsonResultValue);

            Assert.True(response.Success);
            Assert.Equal("Tarefa criada com sucesso.", response.Message);
        }

        [Fact]
        public async Task CreateTask_WhenProjectDoesNotExist_ReturnsBadRequest()
        {
            // Arrange
            CreateTaskDTO createTaskDTO = new CreateTaskDTO
            {
                ProjectId = 1,
                Title = "New Task",
                Description = "Description of the new task",
                DueDate = DateTime.Now.AddDays(5).ToString("yyyy-MM-dd"),
                TaskItemPriority = TaskItemPriority.Medium
            };
            _projectRepoMock.Setup(repo => repo.GetProjectById(It.IsAny<long>())).ReturnsAsync((Project)null);

            // Act
            TmException result = await Assert.ThrowsAsync<TmException>(() => _service.CreateTask(createTaskDTO));

            // Assert
            string jsonResultValue = JsonConvert.SerializeObject(result);
            TasksResponse response = JsonConvert.DeserializeObject<TasksResponse>(jsonResultValue);

            Assert.Equal(400, (int)result.StatusCode);
            Assert.Equal("Projeto inexistente.", response.Message);
        }

        [Fact]
        public async Task CreateTask_WhenMaxTasksPerProjectReached_ReturnsBadRequest()
        {
            // Arrange
            CreateTaskDTO createTaskDTO = new CreateTaskDTO
            {
                ProjectId = 1,
                Title = "New Task",
                Description = "Description of the new task",
                DueDate = DateTime.Now.AddDays(5).ToString("yyyy-MM-dd"),
                TaskItemPriority = TaskItemPriority.Medium
            };

            Project project = new Project("Project 1", "Description of the project", DateTime.Now.AddDays(10), new User { Id = 1, Type = UserType.Manager }) { Id = 1 };

            _projectRepoMock.Setup(repo => repo.GetProjectById(It.IsAny<long>())).ReturnsAsync(project);
            _taskRepoMock.Setup(repo => repo.CountTaskItemsByProjectId(It.IsAny<long>())).ReturnsAsync(20);

            // Act
            TmException result = await Assert.ThrowsAsync<TmException>(() => _service.CreateTask(createTaskDTO));

            // Assert
            string jsonResultValue = JsonConvert.SerializeObject(result);
            TasksResponse response = JsonConvert.DeserializeObject<TasksResponse>(jsonResultValue);

            Assert.Equal(400, (int)result.StatusCode);
            Assert.Equal("Número máximo de tarefas por projeto atingido.", response.Message);
        }

        [Fact]
        public async Task UpdateTask_WhenValidUpdate_ReturnsOk()
        {
            // Arrange
            UpdateTaskDTO updateTaskDTO = new UpdateTaskDTO
            {
                TaskItemId = 1,
                Description = "Updated Description",
                Status = TaskItemStatus.Completed
            };

            Project project = new Project("Project 1", "Description of the project", DateTime.Now, new User { Id = 1, Type = UserType.Manager }) { Id = 1 };

            TaskItem existingTask = new TaskItem
            {
                Id = 1,
                Description = "Old Description",
                Status = TaskItemStatus.InProgress,
                Project = project
            };

            _taskRepoMock.Setup(repo => repo.GetTaskItemById(1)).ReturnsAsync(existingTask);
            _projectRepoMock.Setup(repo => repo.GetProjectById(1)).ReturnsAsync(project);
            _taskRepoMock.Setup(repo => repo.Save(It.IsAny<TaskItem>())).Returns(Task.CompletedTask);
            _taskHistoricRepoMock.Setup(repo => repo.Save(It.IsAny<TaskItemHistoric>())).Returns(Task.CompletedTask);

            // Act
            TaskResponseDTO result = await _service.UpdateTask(updateTaskDTO);

            string jsonResultValue = JsonConvert.SerializeObject(result);
            TasksResponse response = JsonConvert.DeserializeObject<TasksResponse>(jsonResultValue);

            Assert.True(response.Success);
            Assert.Equal("Tarefa atualizada com sucesso.", response.Message);
        }

        [Fact]
        public async Task UpdateTask_WhenTaskItemDoesNotExist_ReturnsBadRequest()
        {
            // Arrange
            UpdateTaskDTO updateTaskDTO = new UpdateTaskDTO { TaskItemId = 1 };

            _taskRepoMock.Setup(repo => repo.GetTaskItemById(1)).ReturnsAsync((TaskItem)null);

            // Act
            TmException result = await Assert.ThrowsAsync<TmException>(() => _service.UpdateTask(updateTaskDTO));

            // Assert
            string jsonResultValue = JsonConvert.SerializeObject(result);
            TasksResponse response = JsonConvert.DeserializeObject<TasksResponse>(jsonResultValue);

            Assert.Equal(400, (int)result.StatusCode);
            Assert.Equal("Tarefa inexistente.", response.Message);
        }

        [Fact]
        public async Task UpdateTask_WhenNoChangesDetected_ReturnsBadRequest()
        {
            // Arrange
            UpdateTaskDTO updateTaskDTO = new UpdateTaskDTO
            {
                TaskItemId = 1,
                Description = "Same Description",
                Status = TaskItemStatus.InProgress
            };

            Project project = new Project("Project 1", "Description of the project", DateTime.Now, new User { Id = 1, Type = UserType.Manager }) { Id = 1 };

            TaskItem existingTask = new TaskItem
            {
                Id = 1,
                Description = "Same Description",
                Status = TaskItemStatus.InProgress,
                Project = project
            };

            _taskRepoMock.Setup(repo => repo.GetTaskItemById(1)).ReturnsAsync(existingTask);

            // Act
            TmException result = await Assert.ThrowsAsync<TmException>(() => _service.UpdateTask(updateTaskDTO));

            // Assert
            string jsonResultValue = JsonConvert.SerializeObject(result);
            TasksResponse response = JsonConvert.DeserializeObject<TasksResponse>(jsonResultValue);

            Assert.Equal(400, (int)result.StatusCode);
            Assert.Equal("Nenhuma alteração detectada. Modifique algum campo para atualizar a tarefa.", response.Message);
        }

        [Fact]
        public async Task UpdateTask_WhenProjectNotFound_ReturnsBadRequest()
        {
            // Arrange
            UpdateTaskDTO updateTaskDTO = new UpdateTaskDTO
            {
                TaskItemId = 1,
                Description = "New Desc",
                Status = TaskItemStatus.Completed
            };

            Project project = new Project("Project 1", "Description of the project", DateTime.Now, new User { Id = 1, Type = UserType.Manager }) { Id = 1 };

            TaskItem taskItem = new TaskItem
            {
                Id = 1,
                Description = "Old Desc",
                Status = TaskItemStatus.InProgress,
                Project = project
            };

            _taskRepoMock.Setup(repo => repo.GetTaskItemById(1)).ReturnsAsync(taskItem);
            _projectRepoMock.Setup(repo => repo.GetProjectById(1)).ReturnsAsync((Project)null);

            // Act
            TmException result = await Assert.ThrowsAsync<TmException>(() => _service.UpdateTask(updateTaskDTO));

            // Assert
            string jsonResultValue = JsonConvert.SerializeObject(result);
            TasksResponse response = JsonConvert.DeserializeObject<TasksResponse>(jsonResultValue);

            Assert.Equal(400, (int)result.StatusCode);
            Assert.Equal("Projeto inexistente.", response.Message);
        }

        [Fact]
        public async Task DeleteTaskItem_WhenTaskExists_ReturnsOk()
        {
            // Arrange
            long taskItemId = 1;
            Project project = new Project("Project 1", "Description of the project", DateTime.Now.AddDays(5), new User { Id = 1, Type = UserType.Manager }) { Id = 1 };

            TaskItem taskItem = new TaskItem
            {
                Id = taskItemId,
                Title = "Task 1",
                Project = project,
                CreatedAt = DateTime.Now.AddDays(-1),
                UpdatedAt = DateTime.Now.AddDays(-1),
                TaskItemPriority = TaskItemPriority.Medium,
                Status = TaskItemStatus.InProgress
            };

            _taskRepoMock.Setup(repo => repo.GetTaskItemById(taskItemId)).ReturnsAsync(taskItem);
            _projectRepoMock.Setup(repo => repo.GetProjectById(project.Id.Value)).ReturnsAsync(project);
            _taskRepoMock.Setup(repo => repo.DeleteTaskItem(It.IsAny<TaskItem>())).Returns(Task.CompletedTask);
            _taskHistoricRepoMock.Setup(repo => repo.Save(It.IsAny<TaskItemHistoric>())).Returns(Task.CompletedTask);

            // Act
            TaskResponseDTO result = await _service.DeleteTaskItem(taskItemId);

            // Assert
            string jsonResultValue = JsonConvert.SerializeObject(result);
            TasksResponse response = JsonConvert.DeserializeObject<TasksResponse>(jsonResultValue);

            Assert.True(response.Success);
            Assert.Equal("Tarefa deletada com sucesso.", response.Message);
        }

        [Fact]
        public async Task DeleteTaskItem_WhenTaskDoesNotExist_ReturnsBadRequest()
        {
            // Arrange
            long taskItemId = 1;
            _taskRepoMock.Setup(repo => repo.GetTaskItemById(taskItemId)).ReturnsAsync((TaskItem)null);

            // Act
            TmException result = await Assert.ThrowsAsync<TmException>(() => _service.DeleteTaskItem(taskItemId));

            // Assert
            string jsonResultValue = JsonConvert.SerializeObject(result);
            TasksResponse response = JsonConvert.DeserializeObject<TasksResponse>(jsonResultValue);

            Assert.Equal(400, (int)result.StatusCode);
            Assert.Equal("Não é possivel deletarmos tarefa pois a mesma nunca foi criada.", response.Message);
        }

        [Fact]
        public async Task DeleteTaskItem_WhenProjectDoesNotExist_ReturnsBadRequest()
        {
            // Arrange
            long taskItemId = 1;
            Project project = null;
            TaskItem taskItem = new TaskItem
            {
                Id = taskItemId,
                Title = "Task 1",
                Project = new Project("Project 1", "Description of the project", DateTime.Now.AddDays(5), new User { Id = 1, Type = UserType.Manager }) { Id = 1 }
            };

            _taskRepoMock.Setup(repo => repo.GetTaskItemById(taskItemId)).ReturnsAsync(taskItem);
            _projectRepoMock.Setup(repo => repo.GetProjectById(taskItem.Project.Id.Value)).ReturnsAsync(project);

            // Act
            TmException result = await Assert.ThrowsAsync<TmException>(() => _service.DeleteTaskItem(taskItemId));

            // Assert
            string jsonResultValue = JsonConvert.SerializeObject(result);
            TasksResponse response = JsonConvert.DeserializeObject<TasksResponse>(jsonResultValue);

            Assert.Equal(400, (int)result.StatusCode);
            Assert.Equal("Projeto inexistente.", response.Message);
        }

        [Fact]
        public async Task CreateTaskComments_WhenValidRequest_ReturnsOk()
        {
            // Arrange
            long taskItemId = 1;
            Project project = new Project("Project 1", "Description of the project", DateTime.Now.AddDays(5), new User { Id = 1, Type = UserType.Manager }) { Id = 1 };

            TaskItem taskItem = new TaskItem
            {
                Id = taskItemId,
                Title = "Task 1",
                Project = project,
                CreatedAt = DateTime.Now.AddDays(-1),
                UpdatedAt = DateTime.Now.AddDays(-1),
                TaskItemPriority = TaskItemPriority.Medium,
                Status = TaskItemStatus.InProgress
            };

            CreateTaskCommentDTO createTaskCommentDTO = new CreateTaskCommentDTO { TaskItemId = taskItemId, Content = "First Comment!" };

            _taskRepoMock.Setup(r => r.GetTaskItemById(taskItemId)).ReturnsAsync(taskItem);
            _projectRepoMock.Setup(r => r.GetProjectById(project.Id.Value)).ReturnsAsync(project);
            _taskCommentRepoMock.Setup(r => r.GetAllTaskCommentByTaskItemId(taskItemId)).ReturnsAsync(new List<TaskComment>());

            // Act
            TaskResponseDTO result = await _service.CreateTaskComments(createTaskCommentDTO);

            // Assert
            string jsonResultValue = JsonConvert.SerializeObject(result);
            TasksResponse response = JsonConvert.DeserializeObject<TasksResponse>(jsonResultValue);

            Assert.True(response.Success);
            Assert.Equal("Comentário inserido com sucesso na Tarefa.", response.Message);
        }

        [Fact]
        public async Task CreateTaskComments_WhenTaskItemDoesNotExist_ReturnsBadRequest()
        {
            // Arrange
            CreateTaskCommentDTO createTaskCommentDTO = new CreateTaskCommentDTO { TaskItemId = 1, Content = "First Comment!" };
            _taskRepoMock.Setup(r => r.GetTaskItemById(createTaskCommentDTO.TaskItemId.Value)).ReturnsAsync((TaskItem)null);

            // Act
            TmException result = await Assert.ThrowsAsync<TmException>(() => _service.CreateTaskComments(createTaskCommentDTO));

            // Assert
            string jsonResultValue = JsonConvert.SerializeObject(result);
            TasksResponse response = JsonConvert.DeserializeObject<TasksResponse>(jsonResultValue);

            Assert.Equal(400, (int)result.StatusCode);
            Assert.Equal("Não é possivel criar um comentário para uma tarefa inexistente.", response.Message);
        }

        [Fact]
        public async Task CreateTaskComments_WhenProjectDoesNotExist_ReturnsBadRequest()
        {
            // Arrange
            long taskItemId = 1;
            Project project = new Project("Project 1", "Description of the project", DateTime.Now.AddDays(5), new User { Id = 1, Type = UserType.Manager }) { Id = 1 };

            TaskItem taskItem = new TaskItem
            {
                Id = taskItemId,
                Title = "Task 1",
                Project = project,
                CreatedAt = DateTime.Now.AddDays(-1),
                UpdatedAt = DateTime.Now.AddDays(-1),
                TaskItemPriority = TaskItemPriority.Medium,
                Status = TaskItemStatus.InProgress
            };

            CreateTaskCommentDTO createTaskCommentDTO = new CreateTaskCommentDTO { TaskItemId = taskItemId, Content = "First Comment!" };

            _taskRepoMock.Setup(r => r.GetTaskItemById(taskItemId)).ReturnsAsync(taskItem);
            _projectRepoMock.Setup(r => r.GetProjectById(taskItem.Project.Id.Value)).ReturnsAsync((Project)null);

            // Act
            TmException result = await Assert.ThrowsAsync<TmException>(() => _service.CreateTaskComments(createTaskCommentDTO));

            // Assert
            string jsonResultValue = JsonConvert.SerializeObject(result);
            TasksResponse response = JsonConvert.DeserializeObject<TasksResponse>(jsonResultValue);

            Assert.Equal(400, (int)result.StatusCode);
            Assert.Equal("Projeto inexistente.", response.Message);
        }

        [Fact]
        public async Task CreateTaskComments_WhenTaskCommentsAreNull_InitializesList()
        {
            // Arrange

            long taskItemId = 1;
            Project project = new Project("Project 1", "Description of the project", DateTime.Now, new User { Id = 1, Type = UserType.Manager }) { Id = 1 };

            TaskItem taskItem = new TaskItem
            {
                Id = taskItemId,
                Title = "Task 1",
                Project = project,
                CreatedAt = DateTime.Now.AddDays(-1),
                UpdatedAt = DateTime.Now.AddDays(-1),
                TaskItemPriority = TaskItemPriority.Medium,
                Status = TaskItemStatus.InProgress
            };

            CreateTaskCommentDTO createTaskCommentDTO = new CreateTaskCommentDTO { TaskItemId = taskItemId, Content = "First Comment!" };

            _taskRepoMock.Setup(r => r.GetTaskItemById(taskItemId)).ReturnsAsync(taskItem);
            _projectRepoMock.Setup(r => r.GetProjectById(project.Id.Value)).ReturnsAsync(project);
            _taskCommentRepoMock.Setup(r => r.GetAllTaskCommentByTaskItemId(taskItemId)).ReturnsAsync((List<TaskComment>)null);

            // Act
            TaskResponseDTO result = await _service.CreateTaskComments(createTaskCommentDTO);

            // Assert
            string jsonResultValue = JsonConvert.SerializeObject(result);
            TasksResponse response = JsonConvert.DeserializeObject<TasksResponse>(jsonResultValue);

            Assert.True(response.Success);
            Assert.Equal("Comentário inserido com sucesso na Tarefa.", response.Message);
        }
    }
}
