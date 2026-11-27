using Inventory.Api.Models;
using Inventory.Api.Services.StockAdjustmentService;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StockAdjustmentsController : ControllerBase
    {
        private readonly IStockAdjustmentService _stockAdjustmentService;

        public StockAdjustmentsController(IStockAdjustmentService stockAdjustmentService)
        {
            _stockAdjustmentService = stockAdjustmentService;
        }

        // GET: api/stockadjustments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StockAdjustment>>> GetStockAdjustments()
        {
            var adjustments = await _stockAdjustmentService.GetAllAdjustmentsAsync();
            return Ok(adjustments);
        }

        // POST: api/stockadjustments
        [HttpPost]
        public async Task<ActionResult<StockAdjustment>> CreateAdjustment([FromBody] StockAdjustment adjustment)
        {
            var result = await _stockAdjustmentService.CreateAdjustmentAsync(adjustment);

            if (result == null)
            {
                return BadRequest(new { message = "Stock adjustment failed. Check if inventory exists or if stock level is sufficient." });
            }

            return CreatedAtAction(nameof(GetStockAdjustments), new { id = result.Id }, result);
        }

        // DELETE: api/stockadjustments/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAdjustment(int id)
        {
            var deleted = await _stockAdjustmentService.DeleteAdjustmentAsync(id);
            if (!deleted)
            {
                return NotFound(new { message = "Stock adjustment not found" });
            }
            return NoContent();
        }
    }
}