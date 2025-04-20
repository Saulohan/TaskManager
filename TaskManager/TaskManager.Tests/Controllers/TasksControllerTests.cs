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

namespace TaskManager.Tests.Controllers
{
    public class TasksControllerTests
    {
        private readonly TasksController _controller;

        private readonly Mock<ITaskService> _taskServiceMock;

        public TasksControllerTests()
        {
            var options = new DbContextOptionsBuilder<TaskManagerContext>()
                .UseInMemoryDatabase("TestDatabase")
                .Options;

            // Mock do ITaskService
            _taskServiceMock = new Mock<ITaskService>();

            _controller = new TasksController(
                _taskServiceMock.Object
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

            TaskResponseDTO responseDTO = new TaskResponseDTO
            {
                Success = true,
                StatusCode = HttpStatusCode.OK,
                Message = "Tarefas recuperadas com sucesso.",
                TaskItems = taskItems
            };

            _taskServiceMock.Setup(service => service.GetAllTasksByProject(projectId)).ReturnsAsync(responseDTO);

            // Act
            IActionResult result = await _controller.GetAllTasksByProject(projectId);

            // Assert
            string jsonResultValue = JsonConvert.SerializeObject(Assert.IsType<ObjectResult>(result).Value);
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

            _taskServiceMock.Setup(service => service.GetAllTasksByProject(projectId))
                           .ThrowsAsync(new TmException("Nenhuma tarefa encontrada.", HttpStatusCode.BadRequest));

            // Act
            IActionResult result = await _controller.GetAllTasksByProject(projectId);

            // Assert
            ObjectResult objectResult = Assert.IsType<ObjectResult>(result);
            string jsonResultValue = JsonConvert.SerializeObject(objectResult.Value);
            TasksResponse response = JsonConvert.DeserializeObject<TasksResponse>(jsonResultValue);

            Assert.Equal((int)HttpStatusCode.BadRequest, objectResult.StatusCode);
            Assert.Equal("Nenhuma tarefa encontrada.", response.Message);
        }

        [Fact]
        public async Task GetAllTasksByProject_WhenUnexpectedErrorOccurs_ReturnsInternalServerError()
        {
            // Arrange
            long projectId = 1;

            _taskServiceMock.Setup(service => service.GetAllTasksByProject(projectId))
                           .ThrowsAsync(new Exception("Erro inesperado"));

            // Act
            IActionResult result = await _controller.GetAllTasksByProject(projectId);

            // Assert
            ObjectResult objectResult = Assert.IsType<ObjectResult>(result);
            string jsonResultValue = JsonConvert.SerializeObject(objectResult.Value);
            TasksResponse response = JsonConvert.DeserializeObject<TasksResponse>(jsonResultValue);

            Assert.Equal((int)HttpStatusCode.InternalServerError, objectResult.StatusCode);
            Assert.Equal("Houve um erro no sistema: Erro inesperado", response.Message.ToString());
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

            TaskResponseDTO responseDTO = new TaskResponseDTO
            {
                Success = true,
                StatusCode = HttpStatusCode.OK,
                Message = "Tarefas recuperadas com sucesso.",
                TaskItems = taskItems
            };

            _taskServiceMock.Setup(service => service.GetAllTasksByProject(projectId)).ReturnsAsync(responseDTO);

            // Act
            IActionResult result = await _controller.GetAllTasksByProject(projectId);

            // Assert
            string jsonResultValue = JsonConvert.SerializeObject(Assert.IsType<ObjectResult>(result).Value);
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

            TaskResponseDTO responseDTO = new TaskResponseDTO
            {
                Success = true,
                Message = "Tarefa criada com sucesso.",
                StatusCode = HttpStatusCode.OK
            };

            _taskServiceMock.Setup(service => service.CreateTask(It.IsAny<CreateTaskDTO>())).ReturnsAsync(responseDTO);

            // Act
            IActionResult result = await _controller.CreateTask(createTaskDTO);

            // Assert
            string jsonResultValue = JsonConvert.SerializeObject(Assert.IsType<ObjectResult>(result).Value);
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
            _taskServiceMock.Setup(service => service.CreateTask(It.IsAny<CreateTaskDTO>())).ThrowsAsync(new TmException("Projeto inexistente.", HttpStatusCode.BadRequest));

            // Act
            IActionResult result = await _controller.CreateTask(createTaskDTO);

            // Assert
            ObjectResult objectResult = Assert.IsType<ObjectResult>(result);

            string jsonResultValue = JsonConvert.SerializeObject(Assert.IsType<ObjectResult>(result).Value);
            TasksResponse response = JsonConvert.DeserializeObject<TasksResponse>(jsonResultValue);

            Assert.Equal(400, objectResult.StatusCode);
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

            _taskServiceMock.Setup(service => service.CreateTask(It.IsAny<CreateTaskDTO>())).ThrowsAsync(new TmException("Número máximo de tarefas por projeto atingido.", HttpStatusCode.BadRequest));

            // Act
            IActionResult result = await _controller.CreateTask(createTaskDTO);

            // Assert
            ObjectResult objectResult = Assert.IsType<ObjectResult>(result);

            string jsonResultValue = JsonConvert.SerializeObject(Assert.IsType<ObjectResult>(result).Value);
            TasksResponse response = JsonConvert.DeserializeObject<TasksResponse>(jsonResultValue);

            Assert.Equal(400, objectResult.StatusCode);
            Assert.Equal("Número máximo de tarefas por projeto atingido.", response.Message);
        }

        [Fact]
        public async Task CreateTask_WhenUnhandledExceptionOccurs_ReturnsInternalServerError()
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

            _taskServiceMock.Setup(service => service.CreateTask(It.IsAny<CreateTaskDTO>())).ThrowsAsync(new Exception("Erro inesperado"));

            // Act
            IActionResult result = await _controller.CreateTask(createTaskDTO);

            // Assert
            ObjectResult objectResult = Assert.IsType<ObjectResult>(result);

            string jsonResultValue = JsonConvert.SerializeObject(Assert.IsType<ObjectResult>(result).Value);
            TasksResponse response = JsonConvert.DeserializeObject<TasksResponse>(jsonResultValue);

            Assert.Equal(500, objectResult.StatusCode);
            Assert.Equal("Houve um erro no sistema: Erro inesperado", response.Message);
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

            TaskResponseDTO responseDTO = new TaskResponseDTO
            {
                Success = true,
                Message = "Tarefa atualizada com sucesso.",
                StatusCode = HttpStatusCode.OK
            };

            _taskServiceMock.Setup(service => service.UpdateTask(It.IsAny<UpdateTaskDTO>())).ReturnsAsync(responseDTO);

            // Act
            IActionResult result = await _controller.UpdateTask(updateTaskDTO);

            string jsonResultValue = JsonConvert.SerializeObject(Assert.IsType<ObjectResult>(result).Value);
            TasksResponse response = JsonConvert.DeserializeObject<TasksResponse>(jsonResultValue);

            Assert.True(response.Success);
            Assert.Equal("Tarefa atualizada com sucesso.", response.Message);
        }

        [Fact]
        public async Task UpdateTask_WhenTaskItemDoesNotExist_ReturnsBadRequest()
        {
            // Arrange
            UpdateTaskDTO updateTaskDTO = new UpdateTaskDTO { TaskItemId = 1 };

            _taskServiceMock.Setup(service => service.UpdateTask(It.IsAny<UpdateTaskDTO>())).ThrowsAsync(new TmException("Tarefa inexistente.", HttpStatusCode.BadRequest));

            // Act
            IActionResult result = await _controller.UpdateTask(updateTaskDTO);

            // Assert
            ObjectResult objectResult = Assert.IsType<ObjectResult>(result);

            string jsonResultValue = JsonConvert.SerializeObject(Assert.IsType<ObjectResult>(result).Value);
            TasksResponse response = JsonConvert.DeserializeObject<TasksResponse>(jsonResultValue);

            Assert.Equal(400, objectResult.StatusCode);
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

            _taskServiceMock.Setup(service => service.UpdateTask(It.IsAny<UpdateTaskDTO>())).ThrowsAsync(new TmException("Nenhuma alteração detectada. Modifique algum campo para atualizar a tarefa.", HttpStatusCode.BadRequest));

            // Act
            IActionResult result = await _controller.UpdateTask(updateTaskDTO);

            // Assert
            ObjectResult objectResult = Assert.IsType<ObjectResult>(result);

            string jsonResultValue = JsonConvert.SerializeObject(Assert.IsType<ObjectResult>(result).Value);
            TasksResponse response = JsonConvert.DeserializeObject<TasksResponse>(jsonResultValue);

            Assert.Equal(400, objectResult.StatusCode);
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

            _taskServiceMock.Setup(service => service.UpdateTask(It.IsAny<UpdateTaskDTO>())).ThrowsAsync(new TmException("Projeto inexistente.", HttpStatusCode.BadRequest));

            // Act
            IActionResult result = await _controller.UpdateTask(updateTaskDTO);

            // Assert
            ObjectResult objectResult = Assert.IsType<ObjectResult>(result);

            string jsonResultValue = JsonConvert.SerializeObject(Assert.IsType<ObjectResult>(result).Value);
            TasksResponse response = JsonConvert.DeserializeObject<TasksResponse>(jsonResultValue);

            Assert.Equal(400, objectResult.StatusCode);
            Assert.Equal("Projeto inexistente.", response.Message);
        }

        [Fact]
        public async Task UpdateTask_WhenUnexpectedErrorOccurs_ReturnsInternalServerError()
        {
            // Arrange
            UpdateTaskDTO updateTaskDTO = new UpdateTaskDTO { TaskItemId = 1 };
            _taskServiceMock.Setup(service => service.UpdateTask(It.IsAny<UpdateTaskDTO>())).ThrowsAsync(new Exception("Algo deu errado."));

            // Act
            IActionResult result = await _controller.UpdateTask(updateTaskDTO);

            // Assert
            ObjectResult objectResult = Assert.IsType<ObjectResult>(result);

            string jsonResultValue = JsonConvert.SerializeObject(Assert.IsType<ObjectResult>(result).Value);
            TasksResponse response = JsonConvert.DeserializeObject<TasksResponse>(jsonResultValue);

            Assert.Equal(500, objectResult.StatusCode);
            Assert.StartsWith("Houve um erro no sistema: Algo deu errado", response.Message);
        }

        [Fact]
        public async Task DeleteTaskItem_WhenTaskExists_ReturnsOk()
        {
            // Arrange
            long taskItemId = 1;

            TaskResponseDTO responseDTO = new TaskResponseDTO
            {
                Success = true,
                Message = "Tarefa deletada com sucesso.",
                StatusCode = HttpStatusCode.OK
            };

            _taskServiceMock.Setup(service => service.DeleteTaskItem(taskItemId)).ReturnsAsync(responseDTO);

            // Act
            IActionResult result = await _controller.DeleteTaskItem(taskItemId);

            // Assert
            string jsonResultValue = JsonConvert.SerializeObject(Assert.IsType<ObjectResult>(result).Value);
            TasksResponse response = JsonConvert.DeserializeObject<TasksResponse>(jsonResultValue);

            Assert.True(response.Success);
            Assert.Equal("Tarefa deletada com sucesso.", response.Message);
        }

        [Fact]
        public async Task DeleteTaskItem_WhenTaskDoesNotExist_ReturnsBadRequest()
        {
            // Arrange
            long taskItemId = 1;
            _taskServiceMock.Setup(service => service.DeleteTaskItem(It.IsAny<long>())).ThrowsAsync(new TmException("Não é possivel deletarmos tarefa pois a mesma nunca foi criada.", HttpStatusCode.BadRequest));

            // Act
            IActionResult result = await _controller.DeleteTaskItem(taskItemId);

            // Assert
            ObjectResult objectResult = Assert.IsType<ObjectResult>(result);

            string jsonResultValue = JsonConvert.SerializeObject(Assert.IsType<ObjectResult>(result).Value);
            TasksResponse response = JsonConvert.DeserializeObject<TasksResponse>(jsonResultValue);

            Assert.Equal(400, objectResult.StatusCode);
            Assert.Equal("Não é possivel deletarmos tarefa pois a mesma nunca foi criada.", response.Message);
        }

        [Fact]
        public async Task DeleteTaskItem_WhenProjectDoesNotExist_ReturnsBadRequest()
        {
            // Arrange
            long taskItemId = 1;

            _taskServiceMock.Setup(service => service.DeleteTaskItem(It.IsAny<long>())).ThrowsAsync(new TmException("Projeto inexistente.", HttpStatusCode.BadRequest));

            // Act
            IActionResult result = await _controller.DeleteTaskItem(taskItemId);

            // Assert
            ObjectResult objectResult = Assert.IsType<ObjectResult>(result);

            string jsonResultValue = JsonConvert.SerializeObject(Assert.IsType<ObjectResult>(result).Value);
            TasksResponse response = JsonConvert.DeserializeObject<TasksResponse>(jsonResultValue);

            Assert.Equal(400, objectResult.StatusCode);
            Assert.Equal("Projeto inexistente.", response.Message);
        }

        [Fact]
        public async Task DeleteTaskItem_WhenUnexpectedErrorOccurs_ReturnsInternalServerError()
        {
            // Arrange
            long taskItemId = 1;
            _taskServiceMock.Setup(service => service.DeleteTaskItem(It.IsAny<long>())).ThrowsAsync(new Exception("Erro inesperado."));

            // Act
            IActionResult result = await _controller.DeleteTaskItem(taskItemId);

            // Assert
            ObjectResult objectResult = Assert.IsType<ObjectResult>(result);

            string jsonResultValue = JsonConvert.SerializeObject(Assert.IsType<ObjectResult>(result).Value);
            TasksResponse response = JsonConvert.DeserializeObject<TasksResponse>(jsonResultValue);

            Assert.Equal(500, objectResult.StatusCode);
            Assert.Contains("Houve um erro no sistema: Erro inesperado", response.Message);
        }

        [Fact]
        public async Task CreateTaskComments_WhenValidRequest_ReturnsOk()
        {
            // Arrange
            long taskItemId = 1;

            TaskResponseDTO responseDTO = new TaskResponseDTO
            {
                Success = true,
                Message = "Comentário inserido com sucesso na Tarefa.",
                StatusCode = HttpStatusCode.OK
            };

            CreateTaskCommentDTO createTaskCommentDTO = new CreateTaskCommentDTO { TaskItemId = taskItemId, Content = "First Comment!" };

            _taskServiceMock.Setup(service => service.CreateTaskComments(It.IsAny<CreateTaskCommentDTO>())).ReturnsAsync(responseDTO);

            // Act
            IActionResult result = await _controller.CreateTaskComments(createTaskCommentDTO);

            // Assert
            string jsonResultValue = JsonConvert.SerializeObject(Assert.IsType<ObjectResult>(result).Value);
            TasksResponse response = JsonConvert.DeserializeObject<TasksResponse>(jsonResultValue);

            Assert.True(response.Success);
            Assert.Equal("Comentário inserido com sucesso na Tarefa.", response.Message);
        }

        [Fact]
        public async Task CreateTaskComments_WhenTaskItemDoesNotExist_ReturnsBadRequest()
        {
            // Arrange
            CreateTaskCommentDTO createTaskCommentDTO = new CreateTaskCommentDTO { TaskItemId = 1, Content = "First Comment!" };
            _taskServiceMock.Setup(service => service.CreateTaskComments(It.IsAny<CreateTaskCommentDTO>())).ThrowsAsync(new TmException("Não é possivel criar um comentário para uma tarefa inexistente.", HttpStatusCode.BadRequest));

            // Act
            IActionResult result = await _controller.CreateTaskComments(createTaskCommentDTO);

            // Assert
            ObjectResult objectResult = Assert.IsType<ObjectResult>(result);

            string jsonResultValue = JsonConvert.SerializeObject(Assert.IsType<ObjectResult>(result).Value);
            TasksResponse response = JsonConvert.DeserializeObject<TasksResponse>(jsonResultValue);

            Assert.Equal(400, objectResult.StatusCode);
            Assert.Equal("Não é possivel criar um comentário para uma tarefa inexistente.", response.Message);
        }

        [Fact]
        public async Task CreateTaskComments_WhenProjectDoesNotExist_ReturnsBadRequest()
        {
            // Arrange
            long taskItemId = 1;

            CreateTaskCommentDTO createTaskCommentDTO = new CreateTaskCommentDTO { TaskItemId = taskItemId, Content = "First Comment!" };

            _taskServiceMock.Setup(service => service.CreateTaskComments(It.IsAny<CreateTaskCommentDTO>())).ThrowsAsync(new TmException("Projeto inexistente.", HttpStatusCode.BadRequest));

            // Act
            IActionResult result = await _controller.CreateTaskComments(createTaskCommentDTO);

            // Assert
            ObjectResult objectResult = Assert.IsType<ObjectResult>(result);

            string jsonResultValue = JsonConvert.SerializeObject(Assert.IsType<ObjectResult>(result).Value);
            TasksResponse response = JsonConvert.DeserializeObject<TasksResponse>(jsonResultValue);

            Assert.Equal(400, objectResult.StatusCode);
            Assert.Equal("Projeto inexistente.", response.Message);
        }

        [Fact]
        public async Task CreateTaskComments_WhenTaskCommentsAreNull_InitializesList()
        {
            // Arrange

            long taskItemId = 1;

            TaskResponseDTO responseDTO = new TaskResponseDTO
            {
                Success = true,
                Message = "Comentário inserido com sucesso na Tarefa.",
                StatusCode = HttpStatusCode.OK
            };

            CreateTaskCommentDTO createTaskCommentDTO = new CreateTaskCommentDTO { TaskItemId = taskItemId, Content = "First Comment!" };

            _taskServiceMock.Setup(service => service.CreateTaskComments(It.IsAny<CreateTaskCommentDTO>())).ReturnsAsync(responseDTO);

            // Act
            IActionResult result = await _controller.CreateTaskComments(createTaskCommentDTO);

            // Assert
            string jsonResultValue = JsonConvert.SerializeObject(Assert.IsType<ObjectResult>(result).Value);
            TasksResponse response = JsonConvert.DeserializeObject<TasksResponse>(jsonResultValue);

            Assert.True(response.Success);
            Assert.Equal("Comentário inserido com sucesso na Tarefa.", response.Message);
        }

        [Fact]
        public async Task CreateTaskComments_WhenExceptionThrown_ReturnsInternalServerError()
        {
            // Arrange
            CreateTaskCommentDTO createTaskCommentDTO = new CreateTaskCommentDTO { TaskItemId = 1, Content = "First Comment!" };
            _taskServiceMock.Setup(service => service.CreateTaskComments(It.IsAny<CreateTaskCommentDTO>())).ThrowsAsync(new Exception("Erro inesperado."));

            // Act
            IActionResult result = await _controller.CreateTaskComments(createTaskCommentDTO);

            // Assert
            ObjectResult objectResult = Assert.IsType<ObjectResult>(result);

            string jsonResultValue = JsonConvert.SerializeObject(Assert.IsType<ObjectResult>(result).Value);
            TasksResponse response = JsonConvert.DeserializeObject<TasksResponse>(jsonResultValue);

            Assert.Equal(500, objectResult.StatusCode);
            Assert.Contains("Houve um erro no sistema: Erro inesperado", response.Message);
        }
    }
}
