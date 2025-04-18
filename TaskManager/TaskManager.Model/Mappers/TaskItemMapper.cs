using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Domain.DTOs;
using TaskManager.Domain.Entities;
using TaskManager.Models.Enums;
using TaskManager.Domain.DTOs;

namespace TaskManager.Domain.Mappers
{
    public static class TaskItemMapper
    {
        public static TaskItem CreateTaskDTOToTaskItem(CreateTaskDTO createTaskDTO)
        {
            return createTaskDTO == null
                ? null
                : new TaskItem
                {
                    Title = createTaskDTO.Title,
                    Description = createTaskDTO.Description,
                    DueDate = createTaskDTO.DueDate,
                    Status = TaskItemStatus.InProgress,
                    TaskItemPriority = createTaskDTO.TaskItemPriority,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Project = createTaskDTO.Project,
                    //CreatedBy = createTaskDTO.User.Id.Value, //Criar KEY
                    //UpdatedBy = createTaskDTO.User.Id.Value //Criar KEY
                };
        }

        public static TaskItem UpdateTaskDTOToTaskItem(UpdateTaskDTO updateTaskDTO, TaskItem taskItem)
        {
            //validar campos para update
            taskItem.Title = updateTaskDTO.Title;
            taskItem.Description = updateTaskDTO.Description;
            taskItem.DueDate = updateTaskDTO.DueDate;
            taskItem.Status = updateTaskDTO.Status;
            taskItem.UpdatedAt = DateTime.UtcNow;
            taskItem.Project = updateTaskDTO.Project;
            //CreatedBy = createTaskDTO.User.Id.Value, //Criar KEY
            //UpdatedBy = createTaskDTO.User.Id.Value //Criar KEY

            return taskItem;
        }
    }
}
