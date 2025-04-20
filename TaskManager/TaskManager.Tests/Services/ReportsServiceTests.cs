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
using TaskManager.Application.Services;
using Microsoft.Build.Evaluation;

namespace TaskManager.Tests.Controllers
{
    public class ReportsServiceTests
    {
        private readonly ReportsService _service;
        private readonly TaskManagerContext _context;
        private readonly Mock<TaskItemRepository> _taskItemRepoMock;
        private readonly Mock<UserRepository> _userRepoMock;
        private readonly Mock<ConfigHelper> _configHelperMock;


        public ReportsServiceTests()
        {
            DbContextOptions<TaskManagerContext> options = new DbContextOptionsBuilder<TaskManagerContext>()
                .UseInMemoryDatabase("TestDatabase")
                .Options;

            _context = new TaskManagerContext(options);

            _taskItemRepoMock = new Mock<TaskItemRepository>(_context);
            _userRepoMock = new Mock<UserRepository>(_context);
            _configHelperMock = new Mock<ConfigHelper>();
            _configHelperMock.Setup(x => x.NumberOfDaysToReport).Returns(30);



            _service = new ReportsService(
                _context,
                _taskItemRepoMock.Object,
                _userRepoMock.Object,
                _configHelperMock.Object
            );
        }

        [Fact]
        public async Task GetAverageCompletedTasks_WhenUserIsNotManager_ReturnsBadRequest()
        {
            // Arrange
            long userId = 1;
            User user = new User { Id = userId, Type = UserType.Client }; // Cliente (não é Manager)

            ReportsResponseDTO responseDTO = new ReportsResponseDTO
            {
                Success = true,
                AverageTasksByUser = 0.663,
                StatusCode = HttpStatusCode.OK,
                TaskCompleted = new List<TaskItem> { new TaskItem() }
            };

            _userRepoMock.Setup(repo => repo.GetUserById(userId)).ReturnsAsync(user);

            // Act
            TmException result = await Assert.ThrowsAsync<TmException>(() => _service.GetAverageCompletedTasks(userId));

            // Assert
            string jsonResultValue = JsonConvert.SerializeObject(result);
            ReportsResponse response = JsonConvert.DeserializeObject<ReportsResponse>(jsonResultValue);

            Assert.Equal((int)HttpStatusCode.BadRequest, (int)result.StatusCode);
            Assert.Equal("Metodo so pode ser acessado por gerentes.", response.Message);
        }

        [Fact]
        public async Task GetAverageCompletedTasks_WhenUserIsManagerAndHasCompletedTasks_ReturnsOk()
        {
            // Arrange
            long userId = 1;
            User user = new User { Id = userId, Type = UserType.Manager };

            List<TaskItem> taskItems = new List<TaskItem>
            {
                new TaskItem
                {
                    Status = TaskItemStatus.Completed,
                    UpdatedAt = DateTime.Now
                },
                new TaskItem
                {
                    Status = TaskItemStatus.Completed,
                    UpdatedAt = DateTime.Now.AddDays(-29)
                },
                new TaskItem
                {
                    Status = TaskItemStatus.InProgress,
                    UpdatedAt = DateTime.Now.AddDays(-5)
                },
                new TaskItem
                {
                    Status = TaskItemStatus.Completed,
                    UpdatedAt = DateTime.Now.AddDays(-31)
                }
            };

            ReportsResponseDTO responseDTO = new ReportsResponseDTO
            {
                Success = true,
                AverageTasksByUser = 0.066666666666666666,
                StatusCode = HttpStatusCode.OK,
                TaskCompleted = new List<TaskItem> { new TaskItem() }
            };

            _userRepoMock.Setup(repo => repo.GetUserById(userId)).ReturnsAsync(user);
            _taskItemRepoMock.Setup(repo =>
                repo.GetCompletedTasksByUserAndDateRange(userId, It.IsAny<DateTime>())
            ).ReturnsAsync(taskItems.Where(t =>
                t.Status == TaskItemStatus.Completed && t.UpdatedAt >= DateTime.Now.AddDays(-30)).ToList());

            // Act
            ReportsResponseDTO result = await _service.GetAverageCompletedTasks(userId);

            // Assert
            string jsonResultValue = JsonConvert.SerializeObject(result);
            ReportsResponse response = JsonConvert.DeserializeObject<ReportsResponse>(jsonResultValue);

            Assert.True(response.Success);
            Assert.Equal(2.0 / 30, response.AverageTasksByUser);
        }

        [Fact]
        public async Task GetAverageCompletedTasks_WhenUserIsManagerAndHasNoCompletedTasks_ReturnsOkWithZeroAverage()
        {
            // Arrange
            long userId = 1;
            User user = new User { Id = userId, Type = UserType.Manager };

            ReportsResponseDTO responseDTO = new ReportsResponseDTO
            {
                Success = true,
                AverageTasksByUser = 0.0,
                StatusCode = HttpStatusCode.OK,
                TaskCompleted = null
            };

            List<TaskItem> taskItems = new List<TaskItem>();

            _userRepoMock.Setup(repo => repo.GetUserById(userId)).ReturnsAsync(user);

            _taskItemRepoMock.Setup(repo =>
                repo.GetCompletedTasksByUserAndDateRange(userId, It.IsAny<DateTime>())
            ).ReturnsAsync(taskItems);

            // Act
            ReportsResponseDTO result = await _service.GetAverageCompletedTasks(userId);

            // Assert
            string jsonResultValue = JsonConvert.SerializeObject(result);
            ReportsResponse response = JsonConvert.DeserializeObject<ReportsResponse>(jsonResultValue);

            Assert.True(response.Success);
            Assert.Equal(0.0, response.AverageTasksByUser);
        }

        [Fact]
        public async Task GetAverageCompletedTasks_WhenUserNotFound_ReturnsBadRequest()
        {
            // Arrange
            long userId = 1;

            _userRepoMock.Setup(repo => repo.GetUserById(userId)).ReturnsAsync((User)null);

            // Act
            TmException result = await Assert.ThrowsAsync<TmException>(() => _service.GetAverageCompletedTasks(userId));

            // Assert
            string jsonResultValue = JsonConvert.SerializeObject(result);
            ReportsResponse response = JsonConvert.DeserializeObject<ReportsResponse>(jsonResultValue);

            Assert.Equal((int)HttpStatusCode.BadRequest, (int)result.StatusCode);
            Assert.Equal("Usuário inexistente.", response.Message);
        }
    }
}
