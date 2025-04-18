using System;
using TaskManager.Models.Enums;

namespace TaskManager.Domain.Entities
{
    public class TaskItemHistoric : BaseModel
    {
        public TaskItem TaskItem { get; set; }

        public TaskItemStatus Status { get; set; }

        public string Description { get; set; }

        public string Comments { get; set; }
    }
}
