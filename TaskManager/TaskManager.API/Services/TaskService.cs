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
using Microsoft.DotNet.Scaffolding.Shared.Messaging;


namespace TaskManager.Application.Services
{
    public class TaskService : ITaskService
    {
        private readonly TaskItemRepository _taskItemRepository;
        private readonly TaskItemHistoricRepository _taskItemHistoricRepository;
        private readonly TaskCommentRepository _taskCommentRepository;
        private readonly ProjectRepository _projectRepository;
        private readonly UserRepository _userRepository;
        private readonly ConfigHelper _configHelper;

        public TaskService(
            TaskItemRepository taskItemRepository,
            ProjectRepository projectRepository,
            TaskItemHistoricRepository taskItemHistoricRepository,
            TaskCommentRepository taskCommentRepository,
            UserRepository userRepository,
            ConfigHelper configHelper)
        {
            _taskItemRepository = taskItemRepository;
            _projectRepository = projectRepository;
            _taskItemHistoricRepository = taskItemHistoricRepository;
            _taskCommentRepository = taskCommentRepository;
            _userRepository = userRepository;
            _configHelper = configHelper;
        }

        public async Task<TaskResponseDTO> GetAllTasksByProject(long projectId)
        {
            List<TaskItem> taskItems = await _taskItemRepository.GetAllTasksByProjectId(projectId);

            if (!taskItems.Any())
                throw new TmException(message: "Nenhuma tarefa encontrada.", statusCode: HttpStatusCode.BadRequest);

            TaskResponseDTO taskResponseDTO = TaskResponseMapper.MapToTaskResponseDTO(success: true, message: "Tarefas recuperadas com sucesso.", statusCode: HttpStatusCode.OK, taskItems: taskItems);

            return taskResponseDTO;
        }

        public async Task<TaskResponseDTO> CreateTask([FromBody] CreateTaskDTO createTaskDTO)
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

            TaskResponseDTO taskResponseDTO = TaskResponseMapper.MapToTaskResponseDTO(success: true, message: "Tarefa criada com sucesso.", statusCode: HttpStatusCode.OK, taskItems: null);

            return taskResponseDTO;
        }

        public async Task<TaskResponseDTO> UpdateTask([FromBody] UpdateTaskDTO updateTaskDTO)
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

            TaskResponseDTO taskResponseDTO = TaskResponseMapper.MapToTaskResponseDTO(success: true, message: "Tarefa atualizada com sucesso.", statusCode: HttpStatusCode.OK, taskItems: null);

            return taskResponseDTO;
        }

        public async Task<TaskResponseDTO> DeleteTaskItem(long taskItemId)
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

            TaskResponseDTO taskResponseDTO = TaskResponseMapper.MapToTaskResponseDTO(success: true, message: "Tarefa deletada com sucesso.", statusCode: HttpStatusCode.OK, taskItems: null);

            return taskResponseDTO;
        }

        public async Task<TaskResponseDTO> CreateTaskComments([FromBody] CreateTaskCommentDTO createTaskCommentDTO)
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

            TaskResponseDTO taskResponseDTO = TaskResponseMapper.MapToTaskResponseDTO(success: true, message: "Comentário inserido com sucesso na Tarefa.", statusCode: HttpStatusCode.OK, taskItems: null);

            return taskResponseDTO;
        }
    }
}
