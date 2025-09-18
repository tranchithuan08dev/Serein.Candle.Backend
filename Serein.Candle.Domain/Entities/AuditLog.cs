﻿using System;
using System.Collections.Generic;

namespace Serein.Candle.Domain.Entities;

public partial class AuditLog
{
    public long AuditId { get; set; }

    public string? TableName { get; set; }

    public string? ActionType { get; set; }

    public string? RecordId { get; set; }

    public string? OldValues { get; set; }

    public string? NewValues { get; set; }

    public int? PerformedBy { get; set; }

    public DateTime PerformedAt { get; set; }

    public virtual User? PerformedByNavigation { get; set; }
}
