using System;
using TaskManager.Models.Enums;

namespace TaskManager.Domain.Entities
{
    public class TaskComment : BaseModel
    {
        public string Content { get; set; }

        public TaskItem TaskItem { get; set; }
    }
}
