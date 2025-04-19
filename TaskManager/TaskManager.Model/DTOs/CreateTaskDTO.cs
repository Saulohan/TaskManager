using MyApp.Attributes;
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
        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        [RegularExpression(@"^\d{2}-\d{2}-\d{4}$", ErrorMessage = "Data inválida. O formato correto é dd-MM-yyyy.")]
        public string DueDate { get; set; }

        [Required]
        [ValidEnumValue(typeof(TaskItemPriority), ErrorMessage = "Prioridade inválida.")]
        public TaskItemPriority? TaskItemPriority { get; set; }

        [Required]
        public long? ProjectId { get; set; }
    }
}
