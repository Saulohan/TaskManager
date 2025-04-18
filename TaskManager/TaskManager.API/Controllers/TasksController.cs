using TaskManager.Database;
using TaskManager.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Domain.Entities;
using TaskManager.Domain.DTOs;
using TaskManager.Domain.Mappers;
using Microsoft.CodeAnalysis;
using TaskManagerAPI.Utils;


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

        public TasksController(TaskManagerContext context, TaskItemRepository taskItemRepository, TaskItemHistoricRepository taskItemHistoricRepository, TaskCommentRepository taskCommentRepository)
        {
            _context = context;
            _taskItemRepository = taskItemRepository;
            _taskItemHistoricRepository = taskItemHistoricRepository;
            _taskCommentRepository = taskCommentRepository;
        }

        // GET /projects/{id}/tasks
        [HttpPost("GetAllTasksByProject")]
        public async Task<IActionResult> GetAllTasksByProject([FromBody] long projectId)
        {
            try
            {
                List<TaskItem> taskItems = await _taskItemRepository.GetAllTasksByProjectId(projectId);

                if (taskItems is null || !taskItems.Any())
                    return Ok(new { Sucess = false, message = "Nenhuma tarefa encontrada." });

                return Ok(new { Sucess = false, message = "Tarefas recuperados com sucesso.", taskItems });
            }
            catch (Exception ex)
            {
                Utils.SaveLogError(ex); // ver se vai ficar ai mesmo.
                return StatusCode(500, new { Sucess = false, mensagem = $"Houve um erro no sistema: {ex.Message}" });
            }
        }

        // POST /projects/{id}/tasks
        [HttpPost("CreateTask")]
        public async Task<IActionResult> CreateTask([FromBody] CreateTaskDTO createTaskDTO)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(createTaskDTO.Title))
                    return BadRequest(new { Sucess = false, mensagem = "Título da tarefa é obrigatório." });
                
                List<TaskItem> taskItem2 = await _taskItemRepository.GetAllTasksByProjectId(createTaskDTO.Project.Id.Value);

                if (taskItem2.Count <= 20) //CRIAR KEY
                    return BadRequest(new { Sucess = false, mensagem = "Numero maximo de tarefas por projeto atingido." });

                TaskItem taskItem = TaskItemMapper.CreateTaskDTOToTaskItem(createTaskDTO);

                await _taskItemRepository.Save(taskItem);

                return Ok(new
                {
                    Sucess = true,
                    mensagem = "Tafefa criada com sucesso."
                });
            }
            catch (Exception ex)
            {
                Utils.SaveLogError(ex); // ver se vai ficar ai mesmo.
                return StatusCode(500, new { Sucess = false, mensagem = $"Houve um erro no sistema: {ex.Message}" });
            }
        }

        // POST /tasks/{id}
        [HttpPost("UpdateTask")]
        public async Task<IActionResult> UpdateTask([FromBody] UpdateTaskDTO updateTaskDTO)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(updateTaskDTO.Title))
                    return BadRequest(new { Sucess = false, mensagem = "Título da tarefa é obrigatório." });

                TaskItem taskItem = await _taskItemRepository.GetTaskItemById(updateTaskDTO.TaskItemId);

                if (taskItem.Status == updateTaskDTO.Status && taskItem.Description == updateTaskDTO.Description)
                    return Ok(new { Sucess = false, message = "Nenhuma alteração detectada. Modifique algum campo para atualizar a tarefa." });

                taskItem = TaskItemMapper.UpdateTaskDTOToTaskItem(updateTaskDTO, taskItem);
                TaskItemHistoric taskItemHistoric = TaskItemHistoricMapper.TaskItemToTaskItemHistoric(taskItem);

                await _taskItemRepository.Save(taskItem);
                await _taskItemHistoricRepository.Save(taskItemHistoric);

                return Ok(new { Sucess = true, mensagem = "Tafefa criada com sucesso." });
            }
            catch (Exception ex)
            {
                Utils.SaveLogError(ex); // ver se vai ficar ai mesmo.
                return StatusCode(500, new { Sucess = false, mensagem = $"Houve um erro no sistema: {ex.Message}" });
            }
        }

        // DELETE /tasks/{id}
        [HttpPost("DeleteTaskItem")]
        public async Task<IActionResult> DeleteTaskItem([FromBody] long taskItemId)
        {
            try
            {

                TaskItem taskItem = await _taskItemRepository.GetTaskItemById(taskItemId);

                if (taskItem is null)
                    return Ok(new { Sucess = false, message = "Não é possivel deletarmos tarefa pois a mesma nunca foi criada." });

                taskItem.UpdatedAt = DateTime.Now;
                taskItem.DeletedAt = DateTime.Now;
                taskItem.UpdatedBy = 0; //criar key

                await _taskItemRepository.DeleteTaskItem(taskItem);

                return Ok(new { Sucess = true, mensagem = "Tafefa criada com sucesso." });
            }
            catch (Exception ex)
            {
                Utils.SaveLogError(ex); // ver se vai ficar ai mesmo.
                return StatusCode(500, new { Sucess = false, mensagem = $"Houve um erro no sistema: {ex.Message}" });
            }
        }

        [HttpPost("CreateTaskComments")]
        public async Task<IActionResult> CreateTaskComments([FromBody] CreateTaskCommentDTO createTaskCommentDTO)
        {
            try
            {

                TaskItem taskItem = await _taskItemRepository.GetTaskItemById(createTaskCommentDTO.TaskItem.Id.Value);

                if (taskItem is null)
                    return BadRequest(new { Sucess = false, mensagem = "Não é possivel criar um comentário para uma tarefa inexistente" });

                TaskComment taskComment = TaskCommentMapper.MapperTaskComment(createTaskCommentDTO);

                await _taskCommentRepository.Save(taskComment);

                List<TaskComment> taskCommentToHistoric = await _taskCommentRepository.GetAllTaskCommentByTaskItemId(taskItem.Id.Value) ?? new List<TaskComment>();

                taskCommentToHistoric.Add(taskComment);

                TaskItemHistoric taskItemHistoric = TaskItemHistoricMapper.MapperCommentsToTaskItemHistoric(taskItem, taskCommentToHistoric);

                await _taskItemHistoricRepository.Save(taskItemHistoric);

                return Ok(new { Sucess = true, mensagem = "Comentário inserido com sucesso na Tarefa." });
            }
            catch (Exception ex)
            {
                Utils.SaveLogError(ex); // ver se vai ficar ai mesmo.
                return StatusCode(500, new { Sucess = false, mensagem = $"Houve um erro no sistema: {ex.Message}" });
            }
        }
    }
}
