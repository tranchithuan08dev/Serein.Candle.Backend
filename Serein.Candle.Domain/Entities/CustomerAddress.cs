using System;
using System.Collections.Generic;

namespace Serein.Candle.Domain.Entities;

public partial class CustomerAddress
{
    public int AddressId { get; set; }

    public int UserId { get; set; }

    public string FullName { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string AddressLine { get; set; } = null!;

    public string? City { get; set; }

    public string? District { get; set; }

    public string? Ward { get; set; }

    public bool IsDefault { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual User User { get; set; } = null!;
}
