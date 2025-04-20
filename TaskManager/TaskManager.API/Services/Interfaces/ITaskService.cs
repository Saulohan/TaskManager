using Microsoft.AspNetCore.Mvc;
using TaskManager.Domain.DTOs;

namespace TaskManager.API.Services.Interfaces
{
    public interface ITaskService
    {
        Task<TaskResponseDTO> GetAllTasksByProject(long projectId);

        Task<TaskResponseDTO> CreateTask([FromBody] CreateTaskDTO createTaskDTO);

        Task<TaskResponseDTO> UpdateTask([FromBody] UpdateTaskDTO updateTaskDTO);

        Task<TaskResponseDTO> DeleteTaskItem(long taskItemId);

        Task<TaskResponseDTO> CreateTaskComments([FromBody] CreateTaskCommentDTO createTaskCommentDTO);
    }
}
