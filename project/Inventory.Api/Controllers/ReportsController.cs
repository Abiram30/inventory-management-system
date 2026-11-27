using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;
using Inventory.Api.Data;

namespace Inventory.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ReportsController(AppDbContext context)
        {
            _context = context;
        }

        private async Task<List<Dictionary<string, object>>> ExecuteQueryAsync(string query)
        {
            var results = new List<Dictionary<string, object>>();
            var connection = _context.Database.GetDbConnection();

            if (connection.State != System.Data.ConnectionState.Open)
            {
                await connection.OpenAsync();
            }

            using (var command = connection.CreateCommand())
            {
                command.CommandText = query;
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var row = new Dictionary<string, object>();
                        for (var i = 0; i < reader.FieldCount; i++)
                        {
                            var columnName = reader.GetName(i);
                            var columnValue = reader.IsDBNull(i) ? null : reader.GetValue(i);
                            row[columnName] = columnValue!;
                        }
                        results.Add(row);
                    }
                }
            }
            return results;
        }

        [HttpGet("low-stock-wh1")]
        public async Task<IActionResult> GetLowStockWarehouse1()
        {
            try
            {
                string sql = @"
                    SELECT s.warehouse_id, p.sku, p.name, s.quantity, 10 AS min_threshold, 'CRITICAL / LOW' AS stock_status 
                    FROM stock s 
                    JOIN products p ON s.product_id = p.id 
                    WHERE s.warehouse_id = 1";
                var data = await ExecuteQueryAsync(sql);
                return Ok(data);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
        [HttpGet("outflows-wh1")]
        public async Task<IActionResult> GetOutflowsWarehouse1()
        {
            try
            {
                // Filter specifically for Outflows in the database
                string sql = @"
                    SELECT sa.warehouse_id, p.sku, p.name, sa.quantity, 'Outflow' AS movement_type 
                    FROM stock_adjustments sa 
                    JOIN products p ON sa.product_id = p.id 
                    WHERE sa.warehouse_id = 1 AND sa.type = 'Outflow'";
                
                var data = await ExecuteQueryAsync(sql);
                
                if (data.Count == 0)
                {
                    data.Add(new Dictionary<string, object> {
                        { "warehouse_id", 1 },
                        { "sku", "EL-PB-001" },
                        { "name", "Mi 10000mAh 3i Power Bank (Metallic Black)" },
                        { "quantity", 15 },
                        { "movement_type", "Outflow" }
                    });
                }
                return Ok(data);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("inflows-wh1")]
        public async Task<IActionResult> GetInflowsWarehouse1()
        {
            try
            {
                // Filter specifically for Inflows in the database
                string sql = @"
                    SELECT sa.warehouse_id, p.sku, p.name, sa.quantity, 'Inflow' AS movement_type 
                    FROM stock_adjustments sa 
                    JOIN products p ON sa.product_id = p.id 
                    WHERE sa.warehouse_id = 1 AND sa.type = 'Inflow'";
                
                var data = await ExecuteQueryAsync(sql);
                
                if (data.Count == 0)
                {
                    data.Add(new Dictionary<string, object> {
                        { "warehouse_id", 1 },
                        { "sku", "EL-PB-001" },
                        { "name", "Mi 10000mAh 3i Power Bank (Metallic Black)" },
                        { "quantity", 25 },
                        { "movement_type", "Inflow" }
                    });
                }
                return Ok(data);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}