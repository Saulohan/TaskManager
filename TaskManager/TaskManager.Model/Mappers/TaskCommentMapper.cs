using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Domain.DTOs;
using TaskManager.Domain.Entities;
using TaskManager.Models.Enums;
using TaskManager.Domain.DTOs;
using static System.Net.Mime.MediaTypeNames;

namespace TaskManager.Domain.Mappers
{
    public static class TaskCommentMapper
    {
        public static TaskComment MapperTaskComment(CreateTaskCommentDTO createTaskCommentDTO, TaskItem taskItem, User user)
        {
            return new TaskComment
                {
                    TaskItem = taskItem,
                    Content = createTaskCommentDTO.Content,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    CreatedBy = user.Id.Value,
                    UpdatedBy = user.Id.Value 
                };
        }
    }
}
