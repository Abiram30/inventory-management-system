using Inventory.Api.Models;

namespace Inventory.Api.Services.WarehouseService
{
    public interface IWarehouseService
    {
        Task<IEnumerable<Warehouse>> GetAllWarehousesAsync();
        Task<Warehouse> CreateWarehouseAsync(Warehouse warehouse);
        
        Task<bool> DeleteWarehouseAsync(int id);
    }
}