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
    public static class TaskResponseMapper
    {
        public static TaskResponseDTO MapToTaskResponseDTO(bool success, string message, HttpStatusCode statusCode, List<TaskItem> taskItems)
        {
            return new TaskResponseDTO
            {
                Success = success,
                Message = message,
                StatusCode = statusCode,
                TaskItems = taskItems
            };
        }
    }

}
