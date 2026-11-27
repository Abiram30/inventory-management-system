using Inventory.Api.Data;
using Inventory.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Api.Services.StockAdjustmentService
{
    public class StockAdjustmentService : IStockAdjustmentService
    {
        private readonly AppDbContext _context;

        public StockAdjustmentService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<StockAdjustment>> GetAllAdjustmentsAsync()
        {
            return await _context.StockAdjustments
                .Include(sa => sa.Product)
                .Include(sa => sa.Warehouse)
                .Include(sa => sa.User)
                .ToListAsync();
        }

        public async Task<StockAdjustment?> CreateAdjustmentAsync(StockAdjustment adjustment)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                adjustment.Timestamp = DateTime.UtcNow;
                _context.StockAdjustments.Add(adjustment);

                var stock = await _context.Stocks
                    .FirstOrDefaultAsync(s => s.WarehouseId == adjustment.WarehouseId && s.ProductId == adjustment.ProductId);

                if (stock == null)
                {
                    if (adjustment.Type.Equals("In", StringComparison.OrdinalIgnoreCase))
                    {
                        stock = new Stock
                        {
                            WarehouseId = adjustment.WarehouseId,
                            ProductId = adjustment.ProductId,
                            Quantity = adjustment.Quantity
                        };
                        _context.Stocks.Add(stock);
                    }
                    else
                    {
                        return null; // Signals failure due to missing inventory
                    }
                }
                else
                {
                    if (adjustment.Type.Equals("In", StringComparison.OrdinalIgnoreCase))
                    {
                        stock.Quantity += adjustment.Quantity;
                    }
                    else if (adjustment.Type.Equals("Out", StringComparison.OrdinalIgnoreCase) || adjustment.Type.Equals("Damage", StringComparison.OrdinalIgnoreCase))
                    {
                        if (stock.Quantity < adjustment.Quantity)
                        {
                            return null; // Signals failure due to insufficient stock
                        }
                        stock.Quantity -= adjustment.Quantity;
                    }
                    else
                    {
                        return null;
                    }

                    _context.Stocks.Update(stock);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return adjustment;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> DeleteAdjustmentAsync(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var adjustment = await _context.StockAdjustments.FindAsync(id);
                if (adjustment == null) return false;

                var stock = await _context.Stocks
                    .FirstOrDefaultAsync(s => s.WarehouseId == adjustment.WarehouseId && s.ProductId == adjustment.ProductId);

                if (stock != null)
                {
                    if (adjustment.Type.Equals("In", StringComparison.OrdinalIgnoreCase))
                    {
                        stock.Quantity = Math.Max(0, stock.Quantity - adjustment.Quantity);
                    }
                    else if (adjustment.Type.Equals("Out", StringComparison.OrdinalIgnoreCase) || adjustment.Type.Equals("Damage", StringComparison.OrdinalIgnoreCase))
                    {
                        stock.Quantity += adjustment.Quantity;
                    }
                    _context.Stocks.Update(stock);
                }

                _context.StockAdjustments.Remove(adjustment);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}