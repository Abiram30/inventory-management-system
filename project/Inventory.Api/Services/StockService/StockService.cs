using Inventory.Api.Data;
using Inventory.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Api.Services.StockService
{
    public class StockService : IStockService
    {
        private readonly AppDbContext _context;

        public StockService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Stock>> GetAllStocksAsync()
        {
            return await _context.Stocks
                .Include(s => s.Product)
                .Include(s => s.Warehouse)
                .ToListAsync();
        }

        public async Task<IEnumerable<Stock>> GetStockByWarehouseIdAsync(int warehouseId)
        {
            return await _context.Stocks
                .Where(s => s.WarehouseId == warehouseId)
                .Include(s => s.Product)
                .Include(s => s.Warehouse)
                .ToListAsync();
        }

        public async Task<Stock> UpsertStockAsync(Stock stockDto)
        {
            var existingStock = await _context.Stocks
                .FirstOrDefaultAsync(s => s.WarehouseId == stockDto.WarehouseId && s.ProductId == stockDto.ProductId);

            if (existingStock == null)
            {
                _context.Stocks.Add(stockDto);
            }
            else
            {
                existingStock.Quantity = stockDto.Quantity;
                _context.Stocks.Update(existingStock);
            }

            await _context.SaveChangesAsync();
            return stockDto;
        }
        public async Task<bool> DeleteStockAsync(int id)
        {
            var stock = await _context.Stocks.FindAsync(id);
            if (stock == null) return false;

            _context.Stocks.Remove(stock);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}