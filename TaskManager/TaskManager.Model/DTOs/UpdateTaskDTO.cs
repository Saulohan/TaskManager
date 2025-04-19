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
    public class UpdateTaskDTO
    {
        [Required]
        public long? TaskItemId { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        [ValidEnumValue(typeof(TaskItemStatus), ErrorMessage = "Status inválido.")]
        public TaskItemStatus? Status { get; set; }
    }
}
