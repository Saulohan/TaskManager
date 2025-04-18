using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Domain.Entities;
using TaskManager.Models.Enums;

namespace TaskManager.Domain.DTOs
{
    public class CreateTaskDTO
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime DueDate { get; set; }

        public TaskItemPriority TaskItemPriority { get; set; }

        public TaskItemStatus Status { get; set; }

        // Validar pois um aponta pro outro
        public virtual Project Project { get; set; }
    }
}
