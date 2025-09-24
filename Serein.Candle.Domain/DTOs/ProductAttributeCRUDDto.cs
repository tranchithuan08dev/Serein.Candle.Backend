using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serein.Candle.Domain.DTOs
{
    public class ProductAttributeCRUDDto
    {
        public int AttributeId { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
