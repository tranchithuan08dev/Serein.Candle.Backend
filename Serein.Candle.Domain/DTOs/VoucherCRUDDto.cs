using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serein.Candle.Domain.DTOs
{
    public class VoucherCRUDDto
    {
        public int VoucherId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int? DiscountPercent { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal? MinOrderAmount { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? MaxUses { get; set; }
        public int UsedCount { get; set; }
        public bool IsActive { get; set; }
    }
}
