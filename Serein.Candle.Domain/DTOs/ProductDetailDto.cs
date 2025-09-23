using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serein.Candle.Domain.DTOs
{
    public class ProductDetailDto
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Ingredients { get; set; } = string.Empty;
        public string BurnTime { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public bool IsActive { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public ICollection<ProductAttributeDto> Attributes { get; set; } = new List<ProductAttributeDto>();
        public ICollection<ProductImageDto> Images { get; set; } = new List<ProductImageDto>();
    }

    public class ProductAttributeDto
    {
        public string AttributeName { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }

    public class ProductImageDto
    {
        public string ImageUrl { get; set; } = string.Empty;
        public bool IsPrimary { get; set; }
    }
}
