using System;
using System.Collections.Generic;

namespace Serein.Candle.Domain.Entities;

public partial class VoucherUser
{
    public int VoucherUserId { get; set; }

    public int VoucherId { get; set; }

    public int UserId { get; set; }

    public bool IsUsed { get; set; }

    public DateTime? UsedAt { get; set; }

    public virtual User User { get; set; } = null!;

    public virtual Voucher Voucher { get; set; } = null!;
}
