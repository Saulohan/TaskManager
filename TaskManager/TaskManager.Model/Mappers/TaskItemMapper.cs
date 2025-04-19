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
    public static class TaskItemMapper
    {
        public static TaskItem CreateTaskDTOToTaskItem(CreateTaskDTO createTaskDTO, Project project)
        {
            return createTaskDTO == null
                ? null
                : new TaskItem
                {
                    Title = createTaskDTO.Title,
                    Description = createTaskDTO.Description,
                    DueDate = Convert.ToDateTime(createTaskDTO.DueDate),
                    Status = TaskItemStatus.InProgress,
                    TaskItemPriority = createTaskDTO.TaskItemPriority.Value,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Project = project,
                    CreatedBy = project.User.Id.Value, 
                    UpdatedBy = project.User.Id.Value
                };
        }

        public static TaskItem UpdateTaskDTOToTaskItem(UpdateTaskDTO updateTaskDTO, TaskItem taskItem, Project project)
        {
            taskItem.Description = updateTaskDTO.Description;
            taskItem.Status = (TaskItemStatus)updateTaskDTO.Status;
            taskItem.UpdatedAt = DateTime.UtcNow;
            taskItem.UpdatedBy = project.User.Id.Value;

            return taskItem;
        }
    }
}
