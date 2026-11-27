using Inventory.Api.Models;
using Inventory.Api.Services.WarehouseService;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WarehousesController : ControllerBase
    {
        private readonly IWarehouseService _warehouseService;

        public WarehousesController(IWarehouseService warehouseService)
        {
            _warehouseService = warehouseService;
        }

        // GET: api/warehouses
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Warehouse>>> GetWarehouses()
        {
            var warehouses = await _warehouseService.GetAllWarehousesAsync();
            return Ok(warehouses);
        }

        // POST: api/warehouses
        [HttpPost]
        public async Task<ActionResult<Warehouse>> PostWarehouse([FromBody] Warehouse warehouse)
        {
            var createdWarehouse = await _warehouseService.CreateWarehouseAsync(warehouse);
            return CreatedAtAction(nameof(GetWarehouses), new { id = createdWarehouse.Id }, createdWarehouse);
        }

        // DELETE: api/warehouses/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWarehouse(int id)
        {
            var deleted = await _warehouseService.DeleteWarehouseAsync(id);
            if (!deleted)
            {
                return NotFound(new { message = "Warehouse not found" });
            }
            return NoContent();
        }
    }
}