using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Domain.DTOs;
using TaskManager.Domain.Entities;
using TaskManager.Domain.DTOs;

namespace TaskManager.Domain.Mappers
{
    public static class ProjectMapper
    {
        public static Project CreateProjectDTOToProduct(CreateProjectDTO createProjectDTO)
        {
            return createProjectDTO == null
                ? null
                : new Project
                {
                    Title = createProjectDTO.Title,
                    Description = createProjectDTO.Description,
                    User = createProjectDTO.User,
                    Status = "CREATEDE", //TROCAR POR ENUM
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    CreatedBy = createProjectDTO.User.Id.Value,
                    UpdatedBy = createProjectDTO.User.Id.Value
                };
        }
    }
}
