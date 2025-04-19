using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Domain.Entities;

namespace TaskManager.Tests.Domain.Responses
{
    public class ProjectsResponse: BaseResponse
    {
        public List<Project> Projects { get; set; }
    }
}
