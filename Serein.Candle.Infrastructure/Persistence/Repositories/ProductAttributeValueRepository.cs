using Serein.Candle.Domain.Entities;
using Serein.Candle.Domain.Interfaces;
using Serein.Candle.Infrastructure.Persistence.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serein.Candle.Infrastructure.Persistence.Repositories
{
    public class ProductAttributeValueRepository : IProductAttributeValueRepository
    {
        private readonly CandleShopDbContext _context;

        public ProductAttributeValueRepository(CandleShopDbContext context)
        {
            _context = context;
        }

        public async Task AddProductAttributeValueAsync(ProductAttributeValue productAttributeValue)
        {
            await _context.ProductAttributeValues.AddAsync(productAttributeValue);
        }
    }
}
