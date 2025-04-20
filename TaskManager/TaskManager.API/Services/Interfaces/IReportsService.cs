using Microsoft.AspNetCore.Mvc;
using TaskManager.Domain.DTOs;

namespace TaskManager.API.Services.Interfaces
{
    public interface IReportsService
    {
        Task<ReportsResponseDTO> GetAverageCompletedTasks(long userId);
    }
}
