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

namespace TaskManager.Tests.Controllers
{
    public class ReportsControllerTests
    {
        private readonly ReportsController _controller;
        private readonly TaskManagerContext _context;
        private readonly Mock<TaskItemRepository> _taskItemRepoMock;
        private readonly Mock<UserRepository> _userRepoMock;
        private readonly Mock<ConfigHelper> _configHelperMock;

        public ReportsControllerTests()
        {
            DbContextOptions<TaskManagerContext> options = new DbContextOptionsBuilder<TaskManagerContext>()
                .UseInMemoryDatabase("TestDatabase")
                .Options;

            _context = new TaskManagerContext(options);

            _taskItemRepoMock = new Mock<TaskItemRepository>(_context);
            _userRepoMock = new Mock<UserRepository>(_context);
            _configHelperMock = new Mock<ConfigHelper>();
            _configHelperMock.Setup(x => x.NumberOfDaysToReport).Returns(30);

            _controller = new ReportsController(
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

            _userRepoMock.Setup(repo => repo.GetUserById(userId)).ReturnsAsync(user);

            // Act
            IActionResult result = await _controller.GetAverageCompletedTasks(userId);

            // Assert
            ObjectResult objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal((int)HttpStatusCode.BadRequest, objectResult.StatusCode);
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

            _userRepoMock.Setup(repo => repo.GetUserById(userId)).ReturnsAsync(user);
            _taskItemRepoMock.Setup(repo =>
                repo.GetCompletedTasksByUserAndDateRange(userId, It.IsAny<DateTime>())
            ).ReturnsAsync(taskItems.Where(t =>
                t.Status == TaskItemStatus.Completed && t.UpdatedAt >= DateTime.Now.AddDays(-30)).ToList());


            // Act
            IActionResult result = await _controller.GetAverageCompletedTasks(userId);

            // Assert
            string jsonResultValue = JsonConvert.SerializeObject(((OkObjectResult)result).Value);
            ReportsResponse data = JsonConvert.DeserializeObject<ReportsResponse>(jsonResultValue);

            Assert.Equal(2.0 / 30, data.AverageTasksByUser);
        }

        [Fact]
        public async Task GetAverageCompletedTasks_WhenUserIsManagerAndHasNoCompletedTasks_ReturnsOkWithZeroAverage()
        {
            // Arrange
            long userId = 1L;
            User user = new User { Id = userId, Type = UserType.Manager }; 

            List<TaskItem> taskItems = new List<TaskItem>(); 

            _userRepoMock.Setup(repo => repo.GetUserById(userId)).ReturnsAsync(user);

            _taskItemRepoMock.Setup(repo =>
                repo.GetCompletedTasksByUserAndDateRange(userId, It.IsAny<DateTime>())
            ).ReturnsAsync(taskItems);

            // Act
            IActionResult result = await _controller.GetAverageCompletedTasks(userId);

            // Assert
            string jsonResultValue = JsonConvert.SerializeObject(((OkObjectResult)result).Value);
            ReportsResponse data = JsonConvert.DeserializeObject<ReportsResponse>(jsonResultValue);

            Assert.True(data.Success);
            Assert.Equal(0.0, data.AverageTasksByUser);
        }

        [Fact]
        public async Task GetAverageCompletedTasks_WhenUserNotFound_ReturnsBadRequest()
        {
            // Arrange
            long userId = 1;

            _userRepoMock.Setup(repo => repo.GetUserById(userId)).ReturnsAsync((User)null);

            // Act
            IActionResult result = await _controller.GetAverageCompletedTasks(userId);

            // Assert
            ObjectResult objectResult = Assert.IsType<ObjectResult>(result);

            Assert.Equal((int)HttpStatusCode.BadRequest, objectResult.StatusCode);
        }

        [Fact]
        public async Task GetAverageCompletedTasks_WhenUnhandledExceptionOccurs_ReturnsInternalServerError()
        {
            // Arrange
            long userId = 1;
            User user = new User { Id = userId, Type = UserType.Manager };

            _userRepoMock.Setup(repo => repo.GetUserById(userId)).ThrowsAsync(new Exception("Erro inesperado"));

            // Act
            IActionResult result = await _controller.GetAverageCompletedTasks(userId);

            // Assert
            ObjectResult objectResult = Assert.IsType<ObjectResult>(result);

            Assert.Equal((int)HttpStatusCode.InternalServerError, objectResult.StatusCode);
        }
    }
}
