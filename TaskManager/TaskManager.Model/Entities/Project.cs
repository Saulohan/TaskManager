using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Models.Enums;

namespace TaskManager.Domain.Entities
{
    public class Project : BaseModel
    {
        public Project() { }

        public Project(string title, string description, DateTime? dueDate, User user)
        {
            Title = title;
            Description = description;
            DueDate = dueDate;
            User = user;
        }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime? DueDate { get; set; }

        public User User { get; set; }
    }
}