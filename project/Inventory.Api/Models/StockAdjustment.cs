using System;
using System.Collections.Generic;

namespace Inventory.Api.Models;

public partial class StockAdjustment
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public int WarehouseId { get; set; }

    public string Type { get; set; } = null!;

    public int Quantity { get; set; }

    public string? Reason { get; set; }

    public DateTime? Timestamp { get; set; }

    public int UserId { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual User User { get; set; } = null!;

    public virtual Warehouse Warehouse { get; set; } = null!;
}
