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
        public static Project CreateProjectDTOToProduct(CreateProjectDTO createProjectDTO, User user)
        {
            return createProjectDTO == null
                ? null
                : new Project
                {
                    Title = createProjectDTO.Title,
                    Description = createProjectDTO.Description,
                    User = user,
                    DueDate = Convert.ToDateTime(createProjectDTO.DueDate),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    CreatedBy = user.Id.Value,
                    UpdatedBy = user.Id.Value
                };
        }
    }
}
