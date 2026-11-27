using System;
using System.Collections.Generic;

namespace Inventory.Api.Models;

public partial class Warehouse
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Location { get; set; } = null!;

    public virtual ICollection<StockAdjustment> StockAdjustments { get; set; } = new List<StockAdjustment>();

    public virtual ICollection<Stock> Stocks { get; set; } = new List<Stock>();
}
