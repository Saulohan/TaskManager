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
using TaskManager.API.Services.Interfaces;
using TaskManager.Application.Services;
using TaskManager.Domain.Exceptions;

namespace TaskManager.Tests.Controllers
{
    public class ProjectsControllerTests
    {
        private readonly ProjectsController _controller;
        private readonly Mock<IProjectsService> _projectsServiceMock;

        public ProjectsControllerTests()
        {
            DbContextOptions<TaskManagerContext> options = new DbContextOptionsBuilder<TaskManagerContext>()
                .UseInMemoryDatabase("TestDatabase")
                .Options;

            _projectsServiceMock = new Mock<IProjectsService>();

            _controller = new ProjectsController(
                _projectsServiceMock.Object
            );
        }

        [Fact]
        public async Task GetAllProjects_WhenNoProjects_ReturnsBadRequest()
        {
            // Arrange
            List<Project> projects = new List<Project>();

            _projectsServiceMock.Setup(service => service.GetAllProjects()).ThrowsAsync(new TmException("Nenhum projeto encontrado.", HttpStatusCode.BadRequest));

            // Act
            IActionResult result = await _controller.GetAllProjects();

            // Assert
            ObjectResult objectResult = Assert.IsType<ObjectResult>(result);

            string jsonResultValue = JsonConvert.SerializeObject(Assert.IsType<ObjectResult>(result).Value);
            ProjectsResponse response = JsonConvert.DeserializeObject<ProjectsResponse>(jsonResultValue);

            Assert.Equal((int)HttpStatusCode.BadRequest, objectResult.StatusCode);
            Assert.Equal("Nenhum projeto encontrado.", response.Message);
        }

        [Fact]
        public async Task GetAllProjects_WhenProjectsExist_ReturnsOk()
        {
            // Arrange
            User user = new User { Id = 1, Type = UserType.Manager };

            ProjectResponseDTO responseDTO = new ProjectResponseDTO
            {
                Success = true,
                Message = "Projetos recuperados com sucesso.",
                StatusCode = HttpStatusCode.OK,
                Projects = new List<Project> {            
                    new Project("Project 1", "Description 1", DateTime.Now.AddDays(10), user),
                    new Project("Project 2", "Description 2", DateTime.Now.AddDays(15), user)
                }
            };

            _projectsServiceMock.Setup(service => service.GetAllProjects()).ReturnsAsync(responseDTO);

            // Act
            IActionResult result = await _controller.GetAllProjects();

            // Assert
            string jsonResultValue = JsonConvert.SerializeObject(Assert.IsType<ObjectResult>(result).Value);
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

            ProjectResponseDTO responseDTO = new ProjectResponseDTO
            {
                Success = true,
                Message = "Projeto criado com sucesso.",
                StatusCode = HttpStatusCode.OK,
                Projects = new List<Project> {
                    new Project(),
                    new Project()
                }
            };

            _projectsServiceMock.Setup(service => service.CreateProject(It.IsAny<CreateProjectDTO>())).ReturnsAsync(responseDTO);

            // Act
            IActionResult result = await _controller.CreateProject(createProjectDTO);

            // Assert
            string jsonResultValue = JsonConvert.SerializeObject(Assert.IsType<ObjectResult>(result).Value);
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

            _projectsServiceMock.Setup(service => service.CreateProject(It.IsAny<CreateProjectDTO>())).ThrowsAsync(new TmException("Usuário inexistente.", HttpStatusCode.BadRequest));

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
            ProjectResponseDTO responseDTO = new ProjectResponseDTO
            {
                Success = true,
                Message = "Projeto excluído com sucesso.",
                StatusCode = HttpStatusCode.OK,
                Projects = new List<Project> {
                    new Project(),
                    new Project()
                }
            };

            _projectsServiceMock.Setup(service => service.DeleteProject(It.IsAny<long>())).ReturnsAsync(responseDTO);

            // Act
            IActionResult result = await _controller.DeleteProject(projectId);

            // Assert
            string jsonResultValue = JsonConvert.SerializeObject(Assert.IsType<ObjectResult>(result).Value);
            ProjectsResponse response = JsonConvert.DeserializeObject<ProjectsResponse>(jsonResultValue);

            Assert.True(response.Success);
            Assert.Equal("Projeto excluído com sucesso.", response.Message);
        }

        [Fact]
        public async Task DeleteProject_WhenProjectDoesNotExist_ReturnsBadRequest()
        {
            // Arrange
            long projectId = 1;
            _projectsServiceMock.Setup(service => service.DeleteProject(It.IsAny<long>())).ThrowsAsync(new TmException("Projeto inexistente.", HttpStatusCode.BadRequest));

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

            _projectsServiceMock.Setup(service => service.DeleteProject(It.IsAny<long>())).ThrowsAsync(new TmException("Não é possivel remover um projeto com tarefas ativas, finalize as tarefas pendentes antes da remoção.", HttpStatusCode.BadRequest));

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
            _projectsServiceMock.Setup(service => service.DeleteProject(It.IsAny<long>())).ThrowsAsync(new Exception("Erro inesperado."));

            // Act
            IActionResult result = await _controller.DeleteProject(projectId);

            // Assert
            ObjectResult objectResult = Assert.IsType<ObjectResult>(result);

            string jsonResultValue = JsonConvert.SerializeObject(Assert.IsType<ObjectResult>(result).Value);
            ProjectsResponse response = JsonConvert.DeserializeObject<ProjectsResponse>(jsonResultValue);

            Assert.Equal((int)HttpStatusCode.InternalServerError, objectResult.StatusCode);
            Assert.Equal("Erro ao tentar excluir o projeto: Erro inesperado.", response.Message);
        }
    }
}