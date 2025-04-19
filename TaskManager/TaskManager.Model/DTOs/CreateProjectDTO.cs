using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Domain.Entities;

namespace TaskManager.Domain.DTOs
{
    public class CreateProjectDTO
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        [RegularExpression(@"^\d{2}-\d{2}-\d{4}$", ErrorMessage = "Data inválida. O formato correto é yyyy-MM-dd.")]
        public string DueDate { get; set; }

        [Required]
        public long? UserId { get; set; }
    }
}
