using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serein.Candle.Domain.DTOs
{
    public class InsertProductDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        public string SKU { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        public string Ingredients { get; set; } = string.Empty;

        public string BurnTime { get; set; } = string.Empty;

        [Required]
        public decimal Price { get; set; }

        [Required]
        public int CategoryId { get; set; }

        public ICollection<InsertProductAttributeDto> Attributes { get; set; } = new List<InsertProductAttributeDto>();
    }
    public class InsertProductAttributeDto
    {
        [Required]
        public int AttributeId { get; set; }

        [Required]
        public string Value { get; set; } = string.Empty;
    }

}
