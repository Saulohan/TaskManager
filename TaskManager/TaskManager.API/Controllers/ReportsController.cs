using Microsoft.AspNetCore.Mvc;
namespace TaskManager.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ReportsController : Controller
    {
        // GET /reports/performance
        [HttpGet("performance")]
        public async Task<IActionResult> GetPerformanceReport()
        {
            return Ok(new { message = "Relatório de desempenho gerado (apenas gerente)" });
        }

    }
}
