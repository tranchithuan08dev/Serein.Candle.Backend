using System;
using System.Collections.Generic;

namespace Serein.Candle.Domain.Entities;

public partial class Payment
{
    public int PaymentId { get; set; }

    public int OrderId { get; set; }

    public int PaymentMethodId { get; set; }

    public decimal Amount { get; set; }

    public DateTime? PaidAt { get; set; }

    public string Status { get; set; } = null!;

    public string? GatewayResponse { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual PaymentMethod PaymentMethod { get; set; } = null!;
}
