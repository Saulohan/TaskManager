using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Domain.DTOs;
using TaskManager.Domain.Entities;

namespace TaskManager.Domain.Mappers
{
    public static class ProjectResponseMapper
    {
        public static ProjectResponseDTO MapToProjectResponseDTO(bool success, string message, HttpStatusCode statusCode, List<Project> projects)
        {
            return new ProjectResponseDTO
            {
                Success = success,
                Message = message,
                StatusCode = statusCode,
                Projects = projects
            };
        }
    }
}
