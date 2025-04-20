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
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using TaskManager.Models.Enums;


namespace TaskManager.Application.Services
{
    public class ProjectsService : IProjectsService
    {
        private readonly TaskManagerContext _context;
        private readonly ProjectRepository _projectRepository;
        private readonly TaskItemRepository _taskItemRepository;
        private readonly UserRepository _userRepository;

        public ProjectsService(TaskManagerContext context, ProjectRepository projectRepository, TaskItemRepository taskItemRepository, UserRepository userRepository)
        {
            _context = context;
            _projectRepository = projectRepository;
            _taskItemRepository = taskItemRepository;
            _userRepository = userRepository;
        }

        public async Task<ProjectResponseDTO> GetAllProjects()
        {
            List<Project> projects = await _projectRepository.GetAllProjects();

            if (!projects.Any())
                throw new TmException(message: "Nenhum projeto encontrado.", statusCode: HttpStatusCode.BadRequest);

            ProjectResponseDTO projectResponseDTO = ProjectResponseMapper.MapToProjectResponseDTO(success: true, message: "Projetos recuperados com sucesso.", statusCode: HttpStatusCode.OK, projects: projects);

            return projectResponseDTO;
        }

        public async Task<ProjectResponseDTO> CreateProject([FromBody] CreateProjectDTO createProjectDTO)
        {
            User user = await _userRepository.GetUserById(createProjectDTO.UserId.Value)
                ?? throw new TmException(message: "Usuário inexistente.", statusCode: HttpStatusCode.BadRequest);

            Project project = ProjectMapper.CreateProjectDTOToProduct(createProjectDTO, user);

            await _projectRepository.Save(project);

            ProjectResponseDTO projectResponseDTO = ProjectResponseMapper.MapToProjectResponseDTO(success: true, message: "Projeto criado com sucesso.", statusCode: HttpStatusCode.OK, projects: null);

            return projectResponseDTO;
        }

        public async Task<ProjectResponseDTO> DeleteProject(long projectId)
        {
            Project project = await _projectRepository.GetProjectById(projectId)
                ?? throw new TmException(message: "Projeto inexistente.", statusCode: HttpStatusCode.BadRequest);

            List<TaskItem> taskItems = await _taskItemRepository.GetAllTasksByProjectId(projectId);

            if (taskItems.Any())
                throw new TmException(message: "Não é possivel remover um projeto com tarefas ativas, finalize as tarefas pendentes antes da remoção.", statusCode: HttpStatusCode.BadRequest);

            User user = await _userRepository.GetUserById(project.User.Id.Value);

            project.DeletedAt = DateTime.Now;
            project.UpdatedAt = DateTime.Now;
            project.UpdatedBy = user?.Id;

            await _projectRepository.DeleteProject(project);

            ProjectResponseDTO projectResponseDTO = ProjectResponseMapper.MapToProjectResponseDTO(success: true, message: "Projeto excluído com sucesso.", statusCode: HttpStatusCode.OK, projects: null);

            return projectResponseDTO;
        }
    }
}
