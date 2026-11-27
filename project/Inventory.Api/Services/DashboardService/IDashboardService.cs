using Inventory.Api.DTO;

namespace Inventory.Api.Services.DashboardService
{
    public interface IDashboardService
    {
        Task<DashboardMetricsDto> GetMetricsAsync();
    }
}