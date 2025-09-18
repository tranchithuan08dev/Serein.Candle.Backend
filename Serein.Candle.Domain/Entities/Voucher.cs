using System;
using System.Collections.Generic;

namespace Serein.Candle.Domain.Entities;

public partial class Voucher
{
    public int VoucherId { get; set; }

    public string Code { get; set; } = null!;

    public string? Description { get; set; }

    public int? DiscountPercent { get; set; }

    public decimal? DiscountAmount { get; set; }

    public decimal? MinOrderAmount { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public int? MaxUses { get; set; }

    public int UsedCount { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<VoucherUser> VoucherUsers { get; set; } = new List<VoucherUser>();
}
