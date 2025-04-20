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
using TaskManager.API.Services.Interfaces;
using Microsoft.Build.Evaluation;


namespace TaskManager.API.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TasksController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        // GET /projects/{id}/tasks
        [HttpPost("GetAllTasksByProject")]
        public async Task<IActionResult> GetAllTasksByProject(long projectId)
        {
            try
            {
                TaskResponseDTO result = await _taskService.GetAllTasksByProject(projectId);

                return StatusCode(Convert.ToInt32(result.StatusCode), new
                {
                    Success = result.Success,
                    Message = result.Message,
                    TaskItems = result.TaskItems
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

        // POST /projects/{id}/tasks
        [HttpPost("CreateTask")]
        public async Task<IActionResult> CreateTask([FromBody] CreateTaskDTO createTaskDTO)
        {
            try
            {
                TaskResponseDTO result = await _taskService.CreateTask(createTaskDTO);

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

        // POST /tasks/{id}
        [HttpPost("UpdateTask")]
        public async Task<IActionResult> UpdateTask([FromBody] UpdateTaskDTO updateTaskDTO)
        {
            try
            {
                TaskResponseDTO result = await _taskService.UpdateTask(updateTaskDTO);

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

        // DELETE /tasks/{id}
        [HttpPost("DeleteTaskItem")]
        public async Task<IActionResult> DeleteTaskItem(long taskItemId)
        {
            try
            {
                TaskResponseDTO result = await _taskService.DeleteTaskItem(taskItemId);

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

        [HttpPost("CreateTaskComments")]
        public async Task<IActionResult> CreateTaskComments([FromBody] CreateTaskCommentDTO createTaskCommentDTO)
        {
            try
            {
                TaskResponseDTO result = await _taskService.CreateTaskComments(createTaskCommentDTO);

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
                return StatusCode(Convert.ToInt32(HttpStatusCode.InternalServerError), new { SucSuccessess = false, message = $"Houve um erro no sistema: {ex.Message}" });
            }
        }
    }
}
