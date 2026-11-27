using System;
using System.Collections.Generic;

namespace Inventory.Api.Models;

public partial class PurchaseOrder
{
    public int Id { get; set; }

    public string SupplierName { get; set; } = null!;

    public string Status { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<PurchaseOrderLine> PurchaseOrderLines { get; set; } = new List<PurchaseOrderLine>();
}
