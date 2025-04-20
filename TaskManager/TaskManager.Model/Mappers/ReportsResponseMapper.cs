using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Domain.DTOs;
using TaskManager.Domain.Entities;

namespace TaskManager.Domain.Mappers
{
    public static class ReportsResponseMapper
    {
        public static ReportsResponseDTO MapToReportsResponseDTO(bool success, double averageTasksByUser, HttpStatusCode statusCode, List<TaskItem> taskCompleted)
        {
            return new ReportsResponseDTO
            {
                Success = success,
                AverageTasksByUser = averageTasksByUser,
                StatusCode = statusCode,
                TaskCompleted = taskCompleted
            };
        }
    }
}
