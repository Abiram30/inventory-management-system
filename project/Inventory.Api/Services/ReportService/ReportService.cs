using System.Data;
using System.Data.Common;
using System.Text;
using Inventory.Api.Data;
using Inventory.Api.DTO;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Api.Services;

public class ReportService : IReportService
{
    private readonly AppDbContext _context;

    public ReportService(AppDbContext context)
    {
        _context = context;
    }

    private async Task<DbConnection> GetOpenConnectionAsync()
    {
        var conn = (DbConnection)_context.Database.GetDbConnection();
        if (conn.State != ConnectionState.Open)
            await conn.OpenAsync();
        return conn;
    }

    public async Task<IEnumerable<LiveInventoryDto>> GetLiveInventoryAsync(int? warehouseId, string? stockStatus)
    {
        var conn = await GetOpenConnectionAsync();
        using var cmd = conn.CreateCommand();
        var sb = new StringBuilder("SELECT warehouse_id AS WarehouseId, warehouse_name AS WarehouseName, product_id AS ProductId, sku AS Sku, product_name AS ProductName, category AS Category, quantity AS Quantity, low_stock_threshold AS LowStockThreshold, stock_status AS StockStatus FROM v_live_inventory WHERE 1=1");
        
        var conditions = new List<string>();
        if (warehouseId.HasValue) { conditions.Add("warehouse_id = @warehouse_id"); AddParam(cmd, "@warehouse_id", warehouseId.Value); }
        if (!string.IsNullOrEmpty(stockStatus)) { conditions.Add("stock_status = @stock_status"); AddParam(cmd, "@stock_status", stockStatus); }

        if (conditions.Count > 0) sb.Append(" AND ").Append(string.Join(" AND ", conditions));
        cmd.CommandText = sb.ToString();

        using var reader = await cmd.ExecuteReaderAsync();
        var list = new List<LiveInventoryDto>();
        while (await reader.ReadAsync())
        {
            list.Add(new LiveInventoryDto {
                WarehouseId = reader.GetInt32(reader.GetOrdinal("WarehouseId")),
                WarehouseName = reader.GetString(reader.GetOrdinal("WarehouseName")),
                ProductId = reader.GetInt32(reader.GetOrdinal("ProductId")),
                Sku = reader.GetString(reader.GetOrdinal("Sku")),
                ProductName = reader.GetString(reader.GetOrdinal("ProductName")),
                Category = reader.GetString(reader.GetOrdinal("Category")),
                Quantity = reader.GetInt32(reader.GetOrdinal("Quantity")),
                LowStockThreshold = reader.GetInt32(reader.GetOrdinal("LowStockThreshold")),
                StockStatus = reader.GetString(reader.GetOrdinal("StockStatus"))
            });
        }
        return list;
    }

    public async Task<IEnumerable<PoFulfillmentDto>> GetPoFulfillmentAsync(string? sku, string? poStatus)
    {
        var conn = await GetOpenConnectionAsync();
        using var cmd = conn.CreateCommand();
        var sb = new StringBuilder("SELECT po_id AS PoId, supplier_name AS SupplierName, po_status AS PoStatus, product_id AS ProductId, sku AS Sku, quantity_ordered AS QuantityOrdered, quantity_received AS QuantityReceived, pending_quantity AS PendingQuantity, fulfillment_pct AS FulfillmentPct FROM v_po_fulfillment WHERE 1=1");

        var conditions = new List<string>();
        if (!string.IsNullOrEmpty(sku)) { conditions.Add("sku = @sku"); AddParam(cmd, "@sku", sku); }
        if (!string.IsNullOrEmpty(poStatus)) { conditions.Add("po_status = @po_status"); AddParam(cmd, "@po_status", poStatus); }

        if (conditions.Count > 0) sb.Append(" AND ").Append(string.Join(" AND ", conditions));
        cmd.CommandText = sb.ToString();

        using var reader = await cmd.ExecuteReaderAsync();
        var list = new List<PoFulfillmentDto>();
        while (await reader.ReadAsync())
        {
            list.Add(new PoFulfillmentDto {
                PoId = reader.GetInt32(reader.GetOrdinal("PoId")),
                SupplierName = reader.GetString(reader.GetOrdinal("SupplierName")),
                PoStatus = reader.GetString(reader.GetOrdinal("PoStatus")),
                ProductId = reader.GetInt32(reader.GetOrdinal("ProductId")),
                Sku = reader.GetString(reader.GetOrdinal("Sku")),
                QuantityOrdered = reader.GetInt32(reader.GetOrdinal("QuantityOrdered")),
                QuantityReceived = reader.GetInt32(reader.GetOrdinal("QuantityReceived")),
                PendingQuantity = reader.GetInt32(reader.GetOrdinal("PendingQuantity")),
                FulfillmentPct = reader.GetDecimal(reader.GetOrdinal("FulfillmentPct"))
            });
        }
        return list;
    }

    public async Task<IEnumerable<MovementLedgerDto>> GetMovementLedgerAsync(int? warehouseId, string? movementType)
    {
        var conn = await GetOpenConnectionAsync();
        using var cmd = conn.CreateCommand();
        var sb = new StringBuilder("SELECT id AS Id, warehouse_id AS WarehouseId, warehouse_name AS WarehouseName, product_id AS ProductId, sku AS Sku, movement_type AS MovementType, quantity AS Quantity, reason AS Reason, user_id AS UserId, username AS Username, timestamp AS Timestamp FROM v_movement_ledger WHERE 1=1");

        var conditions = new List<string>();
        if (warehouseId.HasValue) { conditions.Add("warehouse_id = @warehouse_id"); AddParam(cmd, "@warehouse_id", warehouseId.Value); }
        if (!string.IsNullOrEmpty(movementType)) { conditions.Add("movement_type = @movement_type"); AddParam(cmd, "@movement_type", movementType); }

        if (conditions.Count > 0) sb.Append(" AND ").Append(string.Join(" AND ", conditions));
        sb.Append(" ORDER BY Timestamp DESC LIMIT 50");
        cmd.CommandText = sb.ToString();

        using var reader = await cmd.ExecuteReaderAsync();
        var list = new List<MovementLedgerDto>();
        while (await reader.ReadAsync())
        {
            list.Add(new MovementLedgerDto {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                WarehouseId = reader.GetInt32(reader.GetOrdinal("WarehouseId")),
                WarehouseName = reader.GetString(reader.GetOrdinal("WarehouseName")),
                ProductId = reader.GetInt32(reader.GetOrdinal("ProductId")),
                Sku = reader.GetString(reader.GetOrdinal("Sku")),
                MovementType = reader.GetString(reader.GetOrdinal("MovementType")),
                Quantity = reader.GetInt32(reader.GetOrdinal("Quantity")),
                Reason = reader.IsDBNull(reader.GetOrdinal("Reason")) ? null : reader.GetString(reader.GetOrdinal("Reason")),
                UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                Username = reader.GetString(reader.GetOrdinal("Username")),
                Timestamp = reader.GetDateTime(reader.GetOrdinal("Timestamp"))
            });
        }
        return list;
    }

    private static void AddParam(DbCommand cmd, string name, object value)
    {
        var p = cmd.CreateParameter();
        p.ParameterName = name;
        p.Value = value;
        cmd.Parameters.Add(p);
    }
}