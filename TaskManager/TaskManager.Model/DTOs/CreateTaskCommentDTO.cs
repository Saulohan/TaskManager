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
    public class CreateTaskCommentDTO
    {
        [Required]
        public string Content { get; set; }

        [Required]
        public long? TaskItemId { get; set; }
    }
}
