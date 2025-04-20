using TaskManager.Database;
using TaskManager.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Domain.Entities;
using TaskManager.Domain.DTOs;
using TaskManager.Domain.Mappers;
using Microsoft.CodeAnalysis;
using TaskManagerAPI.Utils;
using Project = TaskManager.Domain.Entities.Project;
using TaskManager.Domain.Exceptions;
using System.Net;
using TaskManager.API.Services.Interfaces;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using TaskManager.Models.Enums;


namespace TaskManager.Application.Services
{
    public class ReportsService : IReportsService
    {
        private readonly TaskManagerContext _context;
        private readonly TaskItemRepository _taskItemRepository;
        private readonly UserRepository _userRepository;
        private readonly ConfigHelper _configHelper;

        public ReportsService(TaskManagerContext context, TaskItemRepository taskItemRepository, UserRepository userRepository, ConfigHelper configHelper)
        {
            _context = context;
            _taskItemRepository = taskItemRepository;
            _userRepository = userRepository;
            _configHelper = configHelper;
        }

        public async Task<ReportsResponseDTO> GetAverageCompletedTasks(long userId)
        {

            User user = await _userRepository.GetUserById(userId)
                ?? throw new TmException(message: "Usuário inexistente.", statusCode: HttpStatusCode.BadRequest);

            if (user.Type is not UserType.Manager)
                throw new TmException(message: "Metodo so pode ser acessado por gerentes.", statusCode: HttpStatusCode.BadRequest);

            DateTime startDate = DateTime.Now.AddDays(-(_configHelper.NumberOfDaysToReport));

            List<TaskItem> taskItems = await _taskItemRepository.GetCompletedTasksByUserAndDateRange(user.Id.Value, startDate);

            ReportsResponseDTO reportsResponseDTO = ReportsResponseMapper.MapToReportsResponseDTO(success: true, averageTasksByUser: Convert.ToDouble(taskItems.Count) / _configHelper.NumberOfDaysToReport, statusCode: HttpStatusCode.OK, taskCompleted: taskItems);

            return reportsResponseDTO;
        }
    }
}
