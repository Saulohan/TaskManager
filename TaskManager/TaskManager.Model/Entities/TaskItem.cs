using System;
using TaskManager.Models.Enums;

namespace TaskManager.Domain.Entities
{
    public class TaskItem : BaseModel
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime DueDate { get; set; }

        public TaskItemPriority TaskItemPriority { get; set; }

        public TaskItemStatus Status { get; set; }

        public Project Project { get; set; }

    }
}
