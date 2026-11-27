namespace Inventory.Api.DTO
{
    public class DashboardMetricsDto
    {
        public int TotalProducts { get; set; }
        public int TotalWarehouses { get; set; }
        public int TotalPurchaseOrders { get; set; }
        public int TotalUsers { get; set; }
        public int LowStockCount { get; set; }
        public int OutOfStockCount { get; set; }
        public decimal TotalSales { get; set; }
        public decimal TotalPurchases { get; set; }
    }
}