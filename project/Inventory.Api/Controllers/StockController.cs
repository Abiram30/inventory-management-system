using Inventory.Api.Models;
using Inventory.Api.Services.StockService;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StockController : ControllerBase
    {
        private readonly IStockService _stockService;

        public StockController(IStockService stockService)
        {
            _stockService = stockService;
        }

        // GET: api/stock
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Stock>>> GetStocks()
        {
            var stocks = await _stockService.GetAllStocksAsync();
            return Ok(stocks);
        }

        // GET: api/stock/warehouse/5
        [HttpGet("warehouse/{warehouseId}")]
        public async Task<ActionResult<IEnumerable<Stock>>> GetStockByWarehouse(int warehouseId)
        {
            var stocks = await _stockService.GetStockByWarehouseIdAsync(warehouseId);
            return Ok(stocks);
        }

        // POST: api/stock
        [HttpPost]
        public async Task<ActionResult<Stock>> UpsertStock([FromBody] Stock stockDto)
        {
            var result = await _stockService.UpsertStockAsync(stockDto);
            return Ok(result);
        }

        // DELETE: api/stock/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStock(int id)
        {
            var deleted = await _stockService.DeleteStockAsync(id);
            if (!deleted)
            {
                return NotFound(new { message = "Stock record not found" });
            }
            return NoContent();
        }
    }
}