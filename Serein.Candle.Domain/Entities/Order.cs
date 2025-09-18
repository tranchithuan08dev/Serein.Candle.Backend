using System;
using System.Collections.Generic;

namespace Serein.Candle.Domain.Entities;
public partial class Order
{
    public int OrderId { get; set; }

    public int? UserId { get; set; }

    public string OrderCode { get; set; } = null!;

    public int StatusId { get; set; }

    public int ShippingAddressId { get; set; }

    public decimal TotalAmount { get; set; }

    public decimal DiscountAmount { get; set; }

    public decimal ShippingFee { get; set; }

    public int PaymentMethodId { get; set; }

    public string? TransactionRef { get; set; }

    public string? Note { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual PaymentMethod PaymentMethod { get; set; } = null!;

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual CustomerAddress ShippingAddress { get; set; } = null!;

    public virtual OrderStatus Status { get; set; } = null!;

    public virtual User? User { get; set; }
}
