using Inventory.Api.Models;
using Inventory.Api.Services.PurchaseOrderService;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurchaseOrdersController : ControllerBase
    {
        private readonly IPurchaseOrderService _purchaseOrderService;

        public PurchaseOrdersController(IPurchaseOrderService purchaseOrderService)
        {
            _purchaseOrderService = purchaseOrderService;
        }

       
       
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PurchaseOrder>>> GetPurchaseOrders()
        {
            var orders = await _purchaseOrderService.GetAllPurchaseOrdersAsync();
            return Ok(orders);
        }

        
        [HttpGet("{id}")]
        public async Task<ActionResult<PurchaseOrder>> GetPurchaseOrder(int id)
        {
            var purchaseOrder = await _purchaseOrderService.GetPurchaseOrderByIdAsync(id);

            if (purchaseOrder == null)
            {
                return NotFound(new { message = "Purchase order not found" });
            }

            return Ok(purchaseOrder);
        }

        
        [HttpPost]
        public async Task<ActionResult<PurchaseOrder>> CreatePurchaseOrder([FromBody] PurchaseOrder purchaseOrder)
        {
            var createdOrder = await _purchaseOrderService.CreatePurchaseOrderAsync(purchaseOrder);
            return CreatedAtAction(nameof(GetPurchaseOrder), new { id = createdOrder.Id }, createdOrder);
        }

        
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePurchaseOrder(int id)
        {
            var deleted = await _purchaseOrderService.DeletePurchaseOrderAsync(id);
            if (!deleted)
            {
                return NotFound(new { message = "Purchase order not found" });
            }
            return NoContent();
        }
    }
}