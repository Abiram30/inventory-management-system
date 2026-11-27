using Inventory.Api.Models;

namespace Inventory.Api.Services.StockAdjustmentService
{
    public interface IStockAdjustmentService
    {
        Task<IEnumerable<StockAdjustment>> GetAllAdjustmentsAsync();
        Task<StockAdjustment?> CreateAdjustmentAsync(StockAdjustment adjustment);
        Task<bool> DeleteAdjustmentAsync(int id);
    }
}