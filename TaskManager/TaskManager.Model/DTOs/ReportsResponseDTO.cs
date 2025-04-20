using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Domain.Entities;

namespace TaskManager.Domain.DTOs
{
    public class ReportsResponseDTO
    {
        public bool Success { get; set; }
        public HttpStatusCode? StatusCode { get; set; }
        public List<TaskItem> TaskCompleted { get; set; }
        public double AverageTasksByUser { get; set; }
    }
}
