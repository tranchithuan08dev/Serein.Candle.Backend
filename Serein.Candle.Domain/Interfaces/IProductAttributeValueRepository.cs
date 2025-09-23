using Serein.Candle.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serein.Candle.Domain.Interfaces
{
    public interface IProductAttributeValueRepository
    {
        Task AddProductAttributeValueAsync(ProductAttributeValue productAttributeValue);
        Task RemoveRangeAsync(IEnumerable<ProductAttributeValue> attributes);
    }
}
