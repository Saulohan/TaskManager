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
        //[Required]
        public string Title { get; set; }

        public string Description { get; set; }

        public string Status { get; set; }

        public virtual User User { get; set; }

        public virtual List<TaskItem> Tasks { get; set; }

    }
}
