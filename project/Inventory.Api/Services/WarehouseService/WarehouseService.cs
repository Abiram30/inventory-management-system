using Inventory.Api.Data;
using Inventory.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Api.Services.WarehouseService
{
    public class WarehouseService : IWarehouseService
    {
        private readonly AppDbContext _context;

        public WarehouseService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Warehouse>> GetAllWarehousesAsync()
        {
            return await _context.Warehouses
            .Include(w => w.StockAdjustments)
            .Include(w => w.Stocks)
            .ToListAsync();
        }

        public async Task<Warehouse> CreateWarehouseAsync(Warehouse warehouse)
        {
            _context.Warehouses.Add(warehouse);
            await _context.SaveChangesAsync();
            return warehouse;
        }
        public async Task<bool> DeleteWarehouseAsync(int id)
{
    var product = await _context.Products.FindAsync(id);
    if (product == null) return false;

    _context.Products.Remove(product);
    await _context.SaveChangesAsync();
    return true;
}
    }
}