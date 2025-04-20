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
using Microsoft.EntityFrameworkCore;
using TaskManager.Tests.Domain.Responses;
using Newtonsoft.Json;
using TaskManager.API.Services.Interfaces;
using TaskManager.Domain.DTOs;
using TaskManager.Domain.Mappers;
using Microsoft.CodeAnalysis;
using TaskManager.Domain.Exceptions;

namespace TaskManager.Tests.Controllers
{
    public class ReportsControllerTests
    {
        private readonly ReportsController _controller;
        private readonly Mock<IReportsService> _reportsService;

        public ReportsControllerTests()
        {
            DbContextOptions<TaskManagerContext> options = new DbContextOptionsBuilder<TaskManagerContext>()
                .UseInMemoryDatabase("TestDatabase")
                .Options;

            _reportsService = new Mock<IReportsService>();

            _controller = new ReportsController(
                _reportsService.Object
            );
        }

        [Fact]
        public async Task GetAverageCompletedTasks_WhenUserIsNotManager_ReturnsBadRequest()
        {
            // Arrange
            long userId = 1;

            ReportsResponseDTO responseDTO = new ReportsResponseDTO
            {
                Success = true,
                AverageTasksByUser = 0.663,
                StatusCode = HttpStatusCode.OK,
                TaskCompleted = new List<TaskItem> { new TaskItem() }
            };

            _reportsService.Setup(service => service.GetAverageCompletedTasks(It.IsAny<long>())).ThrowsAsync(new TmException("Metodo so pode ser acessado por gerentes.", HttpStatusCode.BadRequest));

            // Act
            IActionResult result = await _controller.GetAverageCompletedTasks(userId);

            // Assert
            ObjectResult objectResult = Assert.IsType<ObjectResult>(result);

            string jsonResultValue = JsonConvert.SerializeObject(Assert.IsType<ObjectResult>(result).Value);
            ReportsResponse response = JsonConvert.DeserializeObject<ReportsResponse>(jsonResultValue);

            Assert.Equal((int)HttpStatusCode.BadRequest, objectResult.StatusCode);
            Assert.Equal(400, objectResult.StatusCode);
            Assert.Equal("Metodo so pode ser acessado por gerentes.", response.Message);
        }

        [Fact]
        public async Task GetAverageCompletedTasks_WhenUserIsManagerAndHasCompletedTasks_ReturnsOk()
        {
            // Arrange
            long userId = 1;

            ReportsResponseDTO responseDTO = new ReportsResponseDTO
            {
                Success = true,
                AverageTasksByUser = 0.066666666666666666,
                StatusCode = HttpStatusCode.OK,
                TaskCompleted = new List<TaskItem> { new TaskItem() }
            };

            _reportsService.Setup(service => service.GetAverageCompletedTasks(userId)).ReturnsAsync(responseDTO);

            // Act
            IActionResult result = await _controller.GetAverageCompletedTasks(userId);

            // Assert
            string jsonResultValue = JsonConvert.SerializeObject(Assert.IsType<ObjectResult>(result).Value);
            ReportsResponse response = JsonConvert.DeserializeObject<ReportsResponse>(jsonResultValue);

            Assert.True(response.Success);
            Assert.Equal(2.0 / 30, response.AverageTasksByUser);
        }

        [Fact]
        public async Task GetAverageCompletedTasks_WhenUserIsManagerAndHasNoCompletedTasks_ReturnsOkWithZeroAverage()
        {
            // Arrange
            long userId = 1;

            ReportsResponseDTO responseDTO = new ReportsResponseDTO
            {
                Success = true,
                AverageTasksByUser = 0.0,
                StatusCode = HttpStatusCode.OK,
                TaskCompleted = null
            };

            _reportsService.Setup(service => service.GetAverageCompletedTasks(userId)).ReturnsAsync(responseDTO);

            // Act
            IActionResult result = await _controller.GetAverageCompletedTasks(userId);

            // Assert
            string jsonResultValue = JsonConvert.SerializeObject(Assert.IsType<ObjectResult>(result).Value);
            ReportsResponse response = JsonConvert.DeserializeObject<ReportsResponse>(jsonResultValue);

            Assert.True(response.Success);
            Assert.Equal(0.0, response.AverageTasksByUser);
        }

        [Fact]
        public async Task GetAverageCompletedTasks_WhenUserNotFound_ReturnsBadRequest()
        {
            // Arrange
            long userId = 1;

            _reportsService.Setup(service => service.GetAverageCompletedTasks(It.IsAny<long>())).ThrowsAsync(new TmException("Usuário inexistente.", HttpStatusCode.BadRequest));

            // Act
            IActionResult result = await _controller.GetAverageCompletedTasks(userId);

            // Assert
            ObjectResult objectResult = Assert.IsType<ObjectResult>(result);
            string jsonResultValue = JsonConvert.SerializeObject(Assert.IsType<ObjectResult>(result).Value);
            TasksResponse response = JsonConvert.DeserializeObject<TasksResponse>(jsonResultValue);


            Assert.Equal((int)HttpStatusCode.BadRequest, objectResult.StatusCode);
            Assert.Equal(400, objectResult.StatusCode);
            Assert.Equal("Usuário inexistente.", response.Message);
        }

        [Fact]
        public async Task GetAverageCompletedTasks_WhenUnhandledExceptionOccurs_ReturnsInternalServerError()
        {
            // Arrange
            long userId = 1;

            _reportsService.Setup(service => service.GetAverageCompletedTasks(It.IsAny<long>())).ThrowsAsync(new Exception("Erro inesperado."));

            // Act
            IActionResult result = await _controller.GetAverageCompletedTasks(userId);

            // Assert
            ObjectResult objectResult = Assert.IsType<ObjectResult>(result);
            string jsonResultValue = JsonConvert.SerializeObject(Assert.IsType<ObjectResult>(result).Value);
            TasksResponse response = JsonConvert.DeserializeObject<TasksResponse>(jsonResultValue);

            Assert.Equal(500, objectResult.StatusCode);
            Assert.Contains("Houve um erro no sistema: Erro inesperado", response.Message);
        }
    }
}
