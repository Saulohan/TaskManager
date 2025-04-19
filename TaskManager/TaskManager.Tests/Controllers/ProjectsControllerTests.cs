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
using TaskManager.Tests.Domain.Responses;
using Microsoft.EntityFrameworkCore;
using TaskManager.Domain.DTOs;
using Newtonsoft.Json;

namespace TaskManager.Tests.Controllers
{
    public class ProjectsControllerTests
    {
        private readonly ProjectsController _controller;
        private readonly TaskManagerContext _context;
        private readonly Mock<ProjectRepository> _projectRepoMock;
        private readonly Mock<TaskItemRepository> _taskItemRepoMock;
        private readonly Mock<UserRepository> _userRepoMock;

        public ProjectsControllerTests()
        {
            DbContextOptions<TaskManagerContext> options = new DbContextOptionsBuilder<TaskManagerContext>()
                .UseInMemoryDatabase("TestDatabase")
                .Options;

            _context = new TaskManagerContext(options);

            _projectRepoMock = new Mock<ProjectRepository>(_context);
            _taskItemRepoMock = new Mock<TaskItemRepository>(_context);
            _userRepoMock = new Mock<UserRepository>(_context);

            _controller = new ProjectsController(
                _context,
                _projectRepoMock.Object,
                _taskItemRepoMock.Object,
                _userRepoMock.Object
            );
        }

        [Fact]
        public async Task GetAllProjects_WhenNoProjects_ReturnsBadRequest()
        {
            // Arrange
            List<Project> projects = new List<Project>();
            _projectRepoMock.Setup(repo => repo.GetAllProjects()).ReturnsAsync(projects);

            // Act
            IActionResult result = await _controller.GetAllProjects();

            // Assert
            ObjectResult objectResult = Assert.IsType<ObjectResult>(result);

            Assert.Equal((int)HttpStatusCode.BadRequest, objectResult.StatusCode);
        }

        [Fact]
        public async Task GetAllProjects_WhenProjectsExist_ReturnsOk()
        {
            // Arrange
            User user = new User { Id = 1, Type = UserType.Manager };
            List<Project> projects = new List<Project>
            {
                new Project("Project 1", "Description 1", DateTime.Now.AddDays(10), user),
                new Project("Project 2", "Description 2", DateTime.Now.AddDays(15), user)
            };

            _projectRepoMock.Setup(repo => repo.GetAllProjects()).ReturnsAsync(projects);

            // Act
            IActionResult result = await _controller.GetAllProjects();

            // Assert
            string jsonResultValue = JsonConvert.SerializeObject(((OkObjectResult)result).Value);
            ProjectsResponse response = JsonConvert.DeserializeObject<ProjectsResponse>(jsonResultValue);

            Assert.True(response.Success);
            Assert.Equal("Projetos recuperados com sucesso.", response.Message);
            Assert.Equal(2, response.Projects.Count);
        }

        [Fact]
        public async Task CreateProject_WhenUserExists_ReturnsOk()
        {
            // Arrange
            long userId = 1;
            CreateProjectDTO createProjectDTO = new CreateProjectDTO
            {
                UserId = userId,
                Title = "New Project",
                Description = "New Project Description",
                DueDate = DateTime.Now.AddDays(10).ToString()
            };

            User user = new User { Id = userId, Type = UserType.Manager };

            _userRepoMock.Setup(repo => repo.GetUserById(userId)).ReturnsAsync(user);
            _projectRepoMock.Setup(repo => repo.Save(It.IsAny<Project>())).Returns(Task.CompletedTask);

            // Act
            IActionResult result = await _controller.CreateProject(createProjectDTO);

            // Assert
            string jsonResultValue = JsonConvert.SerializeObject(((OkObjectResult)result).Value);
            ProjectsResponse response = JsonConvert.DeserializeObject<ProjectsResponse>(jsonResultValue);

            Assert.True(response.Success);
            Assert.Equal("Projeto criado com sucesso.", response.Message);
        }

        [Fact]
        public async Task CreateProject_WhenUserDoesNotExist_ReturnsBadRequest()
        {
            // Arrange
            long userId = 1;
            CreateProjectDTO createProjectDTO = new CreateProjectDTO
            {
                UserId = userId,
                Title = "New Project",
                Description = "New Project Description",
                DueDate = DateTime.Now.AddDays(10).ToString()
            };

            _userRepoMock.Setup(repo => repo.GetUserById(userId)).ReturnsAsync((User)null);

            // Act
            IActionResult result = await _controller.CreateProject(createProjectDTO);

            // Assert
            ObjectResult objectResult = Assert.IsType<ObjectResult>(result);

            string jsonResultValue = JsonConvert.SerializeObject(Assert.IsType<ObjectResult>(result).Value);
            ProjectsResponse response = JsonConvert.DeserializeObject<ProjectsResponse>(jsonResultValue);

            Assert.Equal((int)HttpStatusCode.BadRequest, objectResult.StatusCode);
            Assert.Equal("Usuário inexistente.", response.Message);
        }

        [Fact]
        public async Task DeleteProject_WhenProjectExists_ReturnsOk()
        {
            // Arrange
            long projectId = 1;
            Project project = new Project("Project 1", "Description", DateTime.Now.AddDays(10), new User { Id = 1 });
            List<TaskItem> taskItems = new List<TaskItem>();

            _projectRepoMock.Setup(repo => repo.GetProjectById(projectId)).ReturnsAsync(project);
            _taskItemRepoMock.Setup(repo => repo.GetAllTasksByProjectId(projectId)).ReturnsAsync(taskItems);
            _projectRepoMock.Setup(repo => repo.DeleteProject(project)).Returns(Task.CompletedTask);

            // Act
            IActionResult result = await _controller.DeleteProject(projectId);

            // Assert
            string jsonResultValue = JsonConvert.SerializeObject(((OkObjectResult)result).Value);
            ProjectsResponse response = JsonConvert.DeserializeObject<ProjectsResponse>(jsonResultValue);

            Assert.True(response.Success);
            Assert.Equal("Projeto excluído com sucesso.", response.Message);
        }

        [Fact]
        public async Task DeleteProject_WhenProjectDoesNotExist_ReturnsBadRequest()
        {
            // Arrange
            long projectId = 1;
            _projectRepoMock.Setup(repo => repo.GetProjectById(projectId)).ReturnsAsync((Project)null);

            // Act
            IActionResult result = await _controller.DeleteProject(projectId);

            // Assert
            ObjectResult objectResult = Assert.IsType<ObjectResult>(result);

            string jsonResultValue = JsonConvert.SerializeObject(Assert.IsType<ObjectResult>(result).Value);
            ProjectsResponse response = JsonConvert.DeserializeObject<ProjectsResponse>(jsonResultValue);

            Assert.Equal((int)HttpStatusCode.BadRequest, objectResult.StatusCode);
            Assert.Equal("Projeto inexistente.", response.Message);
        }

        [Fact]
        public async Task DeleteProject_WhenProjectHasActiveTasks_ReturnsBadRequest()
        {
            // Arrange
            long projectId = 1;
            Project project = new Project("Project 1", "Description", DateTime.Now.AddDays(10), new User { Id = 1 });
            List<TaskItem> taskItems = new List<TaskItem>
            {
                new TaskItem { Status = TaskItemStatus.InProgress, UpdatedAt = DateTime.Now }
            };

            _projectRepoMock.Setup(repo => repo.GetProjectById(projectId)).ReturnsAsync(project);
            _taskItemRepoMock.Setup(repo => repo.GetAllTasksByProjectId(projectId)).ReturnsAsync(taskItems);

            // Act
            IActionResult result = await _controller.DeleteProject(projectId);

            // Assert
            ObjectResult objectResult = Assert.IsType<ObjectResult>(result);

            string jsonResultValue = JsonConvert.SerializeObject(Assert.IsType<ObjectResult>(result).Value);
            ProjectsResponse response = JsonConvert.DeserializeObject<ProjectsResponse>(jsonResultValue);

            Assert.Equal((int)HttpStatusCode.BadRequest, objectResult.StatusCode);
            Assert.Equal("Não é possivel remover um projeto com tarefas ativas, finalize as tarefas pendentes antes da remoção.", response.Message);
        }

        [Fact]
        public async Task DeleteProject_WhenUnhandledExceptionOccurs_ReturnsInternalServerError()
        {
            // Arrange
            long projectId = 1;
            _projectRepoMock.Setup(repo => repo.GetProjectById(projectId)).ThrowsAsync(new Exception("Erro inesperado"));

            // Act
            IActionResult result = await _controller.DeleteProject(projectId);

            // Assert
            ObjectResult objectResult = Assert.IsType<ObjectResult>(result);

            string jsonResultValue = JsonConvert.SerializeObject(Assert.IsType<ObjectResult>(result).Value);
            ProjectsResponse response = JsonConvert.DeserializeObject<ProjectsResponse>(jsonResultValue);

            Assert.Equal((int)HttpStatusCode.InternalServerError, objectResult.StatusCode);
            Assert.Equal("Erro ao tentar excluir o projeto: Erro inesperado", response.Message);
        }
    }
}
