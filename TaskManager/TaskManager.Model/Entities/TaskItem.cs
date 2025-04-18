using System;
using TaskManager.Models.Enums;

namespace TaskManager.Domain.Entities
{
    public class TaskItem : BaseModel
    {
        //public long TaskItemId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime DueDate { get; set; }

        //Criar ennum
        public TaskItemPriority TaskItemPriority { get; set; }

        public TaskItemStatus Status { get; set; }

        // Validar pois um aponta pro outro
        public virtual Project Project { get; set; }

        public virtual List<TaskComment> Comments { get; set; } = new List<TaskComment>();
    }
}
