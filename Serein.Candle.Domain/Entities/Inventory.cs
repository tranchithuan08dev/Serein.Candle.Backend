﻿using System;
using System.Collections.Generic;

namespace Serein.Candle.Domain.Entities;

public partial class Inventory
{
    public int InventoryId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public int ReorderThreshold { get; set; }

    public DateTime LastUpdated { get; set; }

    public virtual Product Product { get; set; } = null!;
}
