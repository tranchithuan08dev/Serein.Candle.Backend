using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serein.Candle.Domain.DTOs
{
    public class ProductReviewDto
    {
        public int ReviewId { get; set; } 
        public int ProductId { get; set; }
        public int? UserId { get; set; } 
        public byte Rating { get; set; } 
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; } 
        public string UserName { get; set; } = string.Empty;
    }
}
