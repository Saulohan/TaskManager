using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Domain.DTOs;
using TaskManager.Domain.Entities;
using TaskManager.Models.Enums;

namespace TaskManager.Domain.Mappers
{
    public static class TaskItemHistoricMapper
    {
        public static TaskItemHistoric TaskItemToTaskItemHistoric(TaskItem taskItem, User user)
        {
            return  new TaskItemHistoric
            {
                TaskItem = taskItem,
                Status = taskItem.Status,
                Description = taskItem.Description,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                CreatedBy = user.Id.Value,
                UpdatedBy = user.Id.Value,
                DeletedAt = taskItem.DeletedAt
            };
        }

        public static TaskItemHistoric MapperCommentsToTaskItemHistoric(TaskItem taskItem, List<TaskComment> taskComments, User user)
        {
            return  new TaskItemHistoric
            {
                TaskItem = taskItem,
                Status = taskItem.Status,
                Description = taskItem.Description,
                Comments = string.Join("; ", taskComments.Select(taskComment => taskComment.Content)),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                CreatedBy = user.Id.Value,
                UpdatedBy = user.Id.Value 
            };
        }
    }
}
