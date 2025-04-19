using Microsoft.AspNetCore.Mvc;
using System.Net;
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
        private readonly TaskManagerContext _context;
        private readonly TaskItemRepository _taskItemRepository;
        private readonly UserRepository _userRepository;
        private readonly ConfigHelper _configHelper;

        public ReportsController(TaskManagerContext context, TaskItemRepository taskItemRepository, UserRepository userRepository, ConfigHelper configHelper)
        {
            _context = context;
            _taskItemRepository = taskItemRepository;
            _userRepository = userRepository;
            _configHelper = configHelper;
        }

        // GET /reports/performance
        [HttpGet("GetAverageCompletedTasks")]
        public async Task<IActionResult> GetAverageCompletedTasks(long userId)
        {
            try
            {
                int teste = _configHelper.NumberOfDaysToReport;

                User user = await _userRepository.GetUserById(userId)
                    ?? throw new TmException(message: "Usuário inexistente.", statusCode: HttpStatusCode.BadRequest);

                if (user.Type is not UserType.Manager)
                    throw new TmException(message: "Metodo so pode ser acessado por gerentes.", statusCode: HttpStatusCode.BadRequest);

                DateTime startDate = DateTime.Now.AddDays(-(_configHelper.NumberOfDaysToReport));

                List<TaskItem> taskItems = await _taskItemRepository.GetCompletedTasksByUserAndDateRange(user.Id.Value, startDate);

                return Ok(new { Success = true, AverageTasksByUser = Convert.ToDouble(taskItems.Count) / _configHelper.NumberOfDaysToReport, TaskCompleted = taskItems });
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
