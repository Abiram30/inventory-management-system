using Inventory.Api.Data;
using Inventory.Api.DTO;
using Inventory.Api.Services.DashboardService;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Api.Services.DashboardService
{
    public class DashboardService : IDashboardService
    {
        private readonly AppDbContext _context;

        public DashboardService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardMetricsDto> GetMetricsAsync()
        {
            var totalProducts = await _context.Products.CountAsync();
            var totalWarehouses = await _context.Warehouses.CountAsync();
            var totalPurchaseOrders = await _context.PurchaseOrders.CountAsync();
            var totalUsers = await _context.Users.CountAsync();

            var lowStockCount = await _context.Stocks.CountAsync(s => s.Quantity < 5);
            var outOfStockCount = await _context.Stocks.CountAsync(s => s.Quantity == 0);

            return new DashboardMetricsDto
            {
                TotalProducts = totalProducts,
                TotalWarehouses = totalWarehouses,
                TotalPurchaseOrders = totalPurchaseOrders,
                TotalUsers = totalUsers,
                LowStockCount = lowStockCount,
                OutOfStockCount = outOfStockCount,
                TotalSales = 107535.75m,
                TotalPurchases = 129879.43m
            };
        }
    }
}