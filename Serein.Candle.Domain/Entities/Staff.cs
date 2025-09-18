using System;
using System.Collections.Generic;

namespace Serein.Candle.Domain.Entities;

public partial class Staff
{
    public int StaffId { get; set; }

    public int UserId { get; set; }

    public string? EmployeeCode { get; set; }

    public string? Position { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual User User { get; set; } = null!;
}
