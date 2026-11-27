using Inventory.Api.DTO;

namespace Inventory.Api.Services;

public interface IReportService
{
    Task<IEnumerable<LiveInventoryDto>> GetLiveInventoryAsync(int? warehouseId, string? stockStatus);
    Task<IEnumerable<PoFulfillmentDto>> GetPoFulfillmentAsync(string? sku, string? poStatus);
    Task<IEnumerable<MovementLedgerDto>> GetMovementLedgerAsync(int? warehouseId, string? movementType);
}