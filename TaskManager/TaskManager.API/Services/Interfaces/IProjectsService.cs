using Microsoft.AspNetCore.Mvc;
using TaskManager.Domain.DTOs;

namespace TaskManager.API.Services.Interfaces
{
    public interface IProjectsService
    {
        Task<ProjectResponseDTO> GetAllProjects();

        Task<ProjectResponseDTO> CreateProject([FromBody] CreateProjectDTO createProjectDTO);

        Task<ProjectResponseDTO> DeleteProject(long projectId);
    }
}
