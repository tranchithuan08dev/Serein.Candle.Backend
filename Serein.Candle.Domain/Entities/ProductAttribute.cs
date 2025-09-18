using System;
using System.Collections.Generic;

namespace Serein.Candle.Domain.Entities;
public partial class ProductAttribute
{
    public int AttributeId { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<ProductAttributeValue> ProductAttributeValues { get; set; } = new List<ProductAttributeValue>();
}
