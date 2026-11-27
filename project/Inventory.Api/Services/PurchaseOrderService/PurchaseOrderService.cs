using Inventory.Api.Data;
using Inventory.Api.Models;
using Microsoft.EntityFrameworkCore;


namespace Inventory.Api.Services.PurchaseOrderService
{
    public class PurchaseOrderService : IPurchaseOrderService
    {
        private readonly AppDbContext _context;

        public PurchaseOrderService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PurchaseOrder>> GetAllPurchaseOrdersAsync()
        {
            return await _context.PurchaseOrders
                .Include(po => po.PurchaseOrderLines)
                .ThenInclude(line => line.Product)
                .ToListAsync();
        }

        public async Task<PurchaseOrder?> GetPurchaseOrderByIdAsync(int id)
        {
            return await _context.PurchaseOrders
                .Include(po => po.PurchaseOrderLines)
                .ThenInclude(line => line.Product)
                .FirstOrDefaultAsync(po => po.Id == id);
        }

        public async Task<PurchaseOrder> CreatePurchaseOrderAsync(PurchaseOrder purchaseOrder)
        {
            _context.PurchaseOrders.Add(purchaseOrder);
            await _context.SaveChangesAsync();
            return purchaseOrder;
        }
        public async Task<bool> DeletePurchaseOrderAsync(int id)
        {
            var order = await _context.PurchaseOrders.FindAsync(id);
            if (order == null) return false;

            _context.PurchaseOrders.Remove(order);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}