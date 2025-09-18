using System;
using System.Collections.Generic;

namespace Serein.Candle.Domain.Entities;

public partial class RoleType
{
    public int RoleId { get; set; }

    public string RoleName { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
