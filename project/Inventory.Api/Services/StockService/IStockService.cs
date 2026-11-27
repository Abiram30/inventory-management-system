using Inventory.Api.Models;

namespace Inventory.Api.Services.StockService
{
    public interface IStockService
    {
        Task<IEnumerable<Stock>> GetAllStocksAsync();
        Task<IEnumerable<Stock>> GetStockByWarehouseIdAsync(int warehouseId);
        Task<Stock> UpsertStockAsync(Stock stockDto);
        Task<bool> DeleteStockAsync(int id);
    }
}