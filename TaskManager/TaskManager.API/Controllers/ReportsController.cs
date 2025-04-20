using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using System.Net;
using TaskManager.API.Services.Interfaces;
using TaskManager.Database;
using TaskManager.Domain.DTOs;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Exceptions;
using TaskManager.Infrastructure;
using TaskManager.Models.Enums;
using TaskManagerAPI.Utils;

namespace TaskManager.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ReportsController : Controller
    {
        private readonly IReportsService _reportsService;


        public ReportsController(IReportsService reportsService)
        {
            _reportsService = reportsService;
        }

        // GET /reports/performance
        [HttpGet("GetAverageCompletedTasks")]
        public async Task<IActionResult> GetAverageCompletedTasks(long userId)
        {
            try
            {
                ReportsResponseDTO result = await _reportsService.GetAverageCompletedTasks(userId);

                return StatusCode(Convert.ToInt32(result.StatusCode), new
                {
                    Success = result.Success,
                    AverageTasksByUser = result.AverageTasksByUser,
                    TaskCompleted = result.TaskCompleted
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
    }
}
