using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Domain.Entities;

namespace TaskManager.Domain.DTOs
{
    public class TaskResponseDTO
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public HttpStatusCode? StatusCode { get; set; }
        public List<TaskItem> TaskItems { get; set; }
    }
}
