using Inventory.Api.Services.DashboardService;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        
        [HttpGet("metrics")]
        public async Task<IActionResult> GetDashboardMetrics()
        {
            var metrics = await _dashboardService.GetMetricsAsync();
            return Ok(metrics);
        }
    }
}