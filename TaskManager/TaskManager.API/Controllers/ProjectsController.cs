using TaskManager.Database;
using TaskManager.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Domain.Entities;
using TaskManager.Domain.DTOs;
using TaskManager.Domain.Mappers;
using TaskManagerAPI.Utils;
using TaskManager.Domain.Exceptions;
using System.Net;

namespace TaskManager.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProjectsController : ControllerBase
    {
        private readonly TaskManagerContext _context;
        private readonly ProjectRepository _projectRepository;
        private readonly TaskItemRepository _taskItemRepository;
        private readonly UserRepository _userRepository;

        public ProjectsController(TaskManagerContext context, ProjectRepository projectRepository, TaskItemRepository taskItemRepository, UserRepository userRepository)
        {
            _context = context;
            _projectRepository = projectRepository;
            _taskItemRepository = taskItemRepository;
            _userRepository = userRepository;
        }

        // GET /projects
        [HttpGet("GetAllProjects")]
        public async Task<IActionResult> GetAllProjects()
        {
            try
            {
                List<Project> projects = await _projectRepository.GetAllProjects();

                if (!projects.Any())
                    throw new TmException(message: "Nenhum projeto encontrado.", statusCode: HttpStatusCode.BadRequest);

                return Ok(new { Success = true, message = "Projetos recuperados com sucesso.", projects });
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
                User user = await _userRepository.GetUserById(createProjectDTO.UserId.Value)
                    ?? throw new TmException(message: "Usuário inexistente.", statusCode: HttpStatusCode.BadRequest);

                Project project = ProjectMapper.CreateProjectDTOToProduct(createProjectDTO, user);

                await _projectRepository.Save(project);

                return Ok(new { Success = true, message = "Projeto criado com sucesso." });
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
                Project project = await _projectRepository.GetProjectById(projectId)
                    ?? throw new TmException(message: "Projeto inexistente.", statusCode: HttpStatusCode.BadRequest);

                List<TaskItem> taskItems = await _taskItemRepository.GetAllTasksByProjectId(projectId);

                if(taskItems.Any())
                    throw new TmException(message: "Não é possivel remover um projeto com tarefas ativas, finalize as tarefas pendentes antes da remoção.", statusCode: HttpStatusCode.BadRequest);
                
                User user = await _userRepository.GetUserById(project.User.Id.Value);

                project.DeletedAt = DateTime.Now;
                project.UpdatedAt = DateTime.Now;
                project.UpdatedBy = user?.Id ?? 1;

                await _projectRepository.DeleteProject(project);

                return Ok(new { Success = true, message = "Projeto excluído com sucesso." });
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