using TaskManager.Database;
using TaskManager.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Domain.Entities;
using TaskManager.Domain.DTOs;
using TaskManager.Domain.Mappers;
using Microsoft.CodeAnalysis;
using TaskManagerAPI.Utils;
using Project = TaskManager.Domain.Entities.Project;
using TaskManager.Domain.Exceptions;
using System.Net;


namespace TaskManager.API.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly TaskManagerContext _context;
        private readonly TaskItemRepository _taskItemRepository;
        private readonly TaskItemHistoricRepository _taskItemHistoricRepository;
        private readonly TaskCommentRepository _taskCommentRepository;
        private readonly ProjectRepository _projectRepository;
        private readonly UserRepository _userRepository;
        private readonly ConfigHelper _configHelper;

        public TasksController(TaskManagerContext context, TaskItemRepository taskItemRepository, ProjectRepository projectRepository, TaskItemHistoricRepository taskItemHistoricRepository, TaskCommentRepository taskCommentRepository, UserRepository userRepository, ConfigHelper configHelper)
        {
            _context = context;
            _taskItemRepository = taskItemRepository;
            _taskItemHistoricRepository = taskItemHistoricRepository;
            _taskCommentRepository = taskCommentRepository;
            _projectRepository = projectRepository;
            _userRepository = userRepository;
            _configHelper = configHelper;
        }

        // GET /projects/{id}/tasks
        [HttpPost("GetAllTasksByProject")]
        public async Task<IActionResult> GetAllTasksByProject(long projectId)
        {
            try
            {
                List<TaskItem> taskItems = await _taskItemRepository.GetAllTasksByProjectId(projectId);

                if (taskItems is null || !taskItems.Any())
                    throw new TmException(message: "Nenhuma tarefa encontrada.", statusCode: HttpStatusCode.BadRequest);

                return Ok(new { Success = true, message = "Tarefas recuperados com sucesso.", taskItems });
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

        // POST /projects/{id}/tasks
        [HttpPost("CreateTask")]
        public async Task<IActionResult> CreateTask([FromBody] CreateTaskDTO createTaskDTO)
        {
            try
            {

                Project project = await _projectRepository.GetProjectById(createTaskDTO.ProjectId.Value)
                    ?? throw new TmException(message: "Projeto inexistente.", statusCode: HttpStatusCode.BadRequest);

                int countTasksByProject = await _taskItemRepository.CountTaskItemsByProjectId(project.Id.Value);

                if (countTasksByProject >= _configHelper.MaxTasksPerProject) 
                    throw new TmException(message: "Número máximo de tarefas por projeto atingido.", statusCode: HttpStatusCode.BadRequest);

                TaskItem taskItem = TaskItemMapper.CreateTaskDTOToTaskItem(createTaskDTO, project);
                TaskItemHistoric taskItemHistoric = TaskItemHistoricMapper.TaskItemToTaskItemHistoric(taskItem, project.User);

                await _taskItemRepository.Save(taskItem);
                await _taskItemHistoricRepository.Save(taskItemHistoric);

                return Ok(new { Success = true, message = "Tarefa criada com sucesso." });
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

        // POST /tasks/{id}
        [HttpPost("UpdateTask")]
        public async Task<IActionResult> UpdateTask([FromBody] UpdateTaskDTO updateTaskDTO)
        {
            try
            {
                TaskItem taskItem = await _taskItemRepository.GetTaskItemById(updateTaskDTO.TaskItemId.Value)
                    ?? throw new TmException(message: "Tarefa inexistente.", statusCode: HttpStatusCode.BadRequest);

                if (taskItem.Status == updateTaskDTO.Status && taskItem.Description == updateTaskDTO.Description)
                    throw new TmException(message: "Nenhuma alteração detectada. Modifique algum campo para atualizar a tarefa.", statusCode: HttpStatusCode.BadRequest);

                Project project = await _projectRepository.GetProjectById(taskItem.Project.Id.Value)
                    ?? throw new TmException(message: "Projeto inexistente.", statusCode: HttpStatusCode.BadRequest);

                taskItem = TaskItemMapper.UpdateTaskDTOToTaskItem(updateTaskDTO, taskItem, project);
                TaskItemHistoric taskItemHistoric = TaskItemHistoricMapper.TaskItemToTaskItemHistoric(taskItem, project.User);

                await _taskItemRepository.Save(taskItem);
                await _taskItemHistoricRepository.Save(taskItemHistoric);

                return Ok(new { Success = true, message = "Tarefa atualizada com sucesso." });
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

        // DELETE /tasks/{id}
        [HttpPost("DeleteTaskItem")]
        public async Task<IActionResult> DeleteTaskItem(long taskItemId)
        {
            try
            {
                TaskItem taskItem = await _taskItemRepository.GetTaskItemById(taskItemId)
                    ?? throw new TmException(message: "Não é possivel deletarmos tarefa pois a mesma nunca foi criada.", statusCode: HttpStatusCode.BadRequest);

                Project project = await _projectRepository.GetProjectById(taskItem.Project.Id.Value)
                    ?? throw new TmException(message: "Projeto inexistente.", statusCode: HttpStatusCode.BadRequest);

                taskItem.UpdatedAt = DateTime.Now;
                taskItem.DeletedAt = DateTime.Now;
                taskItem.UpdatedBy = project.User.Id.Value;

                TaskItemHistoric taskItemHistoric = TaskItemHistoricMapper.TaskItemToTaskItemHistoric(taskItem, project.User);

                await _taskItemRepository.DeleteTaskItem(taskItem);
                await _taskItemHistoricRepository.Save(taskItemHistoric);

                return Ok(new { Success = true, message = "Tarefa deletada com sucesso." });
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

        [HttpPost("CreateTaskComments")]
        public async Task<IActionResult> CreateTaskComments([FromBody] CreateTaskCommentDTO createTaskCommentDTO)
        {
            try
            {
                TaskItem taskItem = await _taskItemRepository.GetTaskItemById(createTaskCommentDTO.TaskItemId.Value)
                    ?? throw new TmException(message: "Não é possivel criar um comentário para uma tarefa inexistente.", statusCode: HttpStatusCode.BadRequest);

                Project project = await _projectRepository.GetProjectById(taskItem.Project.Id.Value)
                    ?? throw new TmException(message: "Projeto inexistente.", statusCode: HttpStatusCode.BadRequest);

                TaskComment taskComment = TaskCommentMapper.MapperTaskComment(createTaskCommentDTO, taskItem, project.User);

                List<TaskComment> taskCommentToHistoric = await _taskCommentRepository.GetAllTaskCommentByTaskItemId(taskItem.Id.Value) ?? new List<TaskComment>();
                
                await _taskCommentRepository.Save(taskComment);
                taskCommentToHistoric.Add(taskComment);

                TaskItemHistoric taskItemHistoric = TaskItemHistoricMapper.MapperCommentsToTaskItemHistoric(taskItem, taskCommentToHistoric, project.User);

                await _taskItemHistoricRepository.Save(taskItemHistoric);

                return Ok(new { Success = true, message = "Comentário inserido com sucesso na Tarefa." });
            }
            catch (TmException ex)
            {
                Utils.SaveLogError(ex);
                return StatusCode(Convert.ToInt32(ex.StatusCode), new { Success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                Utils.SaveLogError(ex); 
                return StatusCode(Convert.ToInt32(HttpStatusCode.InternalServerError), new { SucSuccessess = false, message = $"Houve um erro no sistema: {ex.Message}" });
            }
        }
    }
}
