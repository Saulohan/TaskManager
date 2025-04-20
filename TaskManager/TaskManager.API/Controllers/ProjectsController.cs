using TaskManager.Database;
using TaskManager.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Domain.Entities;
using TaskManager.Domain.DTOs;
using TaskManager.Domain.Mappers;
using TaskManagerAPI.Utils;
using TaskManager.Domain.Exceptions;
using System.Net;
using TaskManager.API.Services.Interfaces;

namespace TaskManager.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProjectsController : ControllerBase
    {

        private readonly IProjectsService _projectsService;

        public ProjectsController(IProjectsService projectsService)
        {
            _projectsService = projectsService;
        }

        // GET /projects
        [HttpGet("GetAllProjects")]
        public async Task<IActionResult> GetAllProjects()
        {
            try
            {
                ProjectResponseDTO result = await _projectsService.GetAllProjects();

                return StatusCode(Convert.ToInt32(result.StatusCode), new
                {
                    Success = result.Success,
                    Message = result.Message,
                    Projects = result.Projects
                });
            }
            catch (TmException ex)
            {
                Utils.SaveLogError(ex);
                return StatusCode(Convert.ToInt32(ex.StatusCode), new { Success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                Utils.SaveLogError(ex);
                return StatusCode(Convert.ToInt32(HttpStatusCode.InternalServerError), new { Success = false, message = $"Houve um erro no sistema: {ex.Message}" });
            }
        }

        [HttpPost("CreateProject")]
        public async Task<IActionResult> CreateProject([FromBody] CreateProjectDTO createProjectDTO)
        {
            try
            {
                ProjectResponseDTO result = await _projectsService.CreateProject(createProjectDTO);

                return StatusCode(Convert.ToInt32(result.StatusCode), new
                {
                    Success = result.Success,
                    Message = result.Message
                });
            }
            catch (TmException ex)
            {
                Utils.SaveLogError(ex);
                return StatusCode(Convert.ToInt32(ex.StatusCode), new { Success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                Utils.SaveLogError(ex);
                return StatusCode(Convert.ToInt32(HttpStatusCode.InternalServerError), new { Success = false, message = $"Houve um erro no sistema: {ex.Message}" });
            }
        }

        [HttpPost("DeleteProject")]
        public async Task<IActionResult> DeleteProject(long projectId)
        {
            try
            {
                ProjectResponseDTO result = await _projectsService.DeleteProject(projectId);

                return StatusCode(Convert.ToInt32(result.StatusCode), new
                {
                    Success = result.Success,
                    Message = result.Message
                });
            }
            catch (TmException ex)
            {
                Utils.SaveLogError(ex);
                return StatusCode(Convert.ToInt32(ex.StatusCode), new { Success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                Utils.SaveLogError(ex);
                return StatusCode(Convert.ToInt32(HttpStatusCode.InternalServerError), new { Success = false, message = $"Erro ao tentar excluir o projeto: {ex.Message}" });
            }
        }

    }
}