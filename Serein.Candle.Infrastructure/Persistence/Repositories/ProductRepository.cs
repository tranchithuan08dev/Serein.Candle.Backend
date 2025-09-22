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
    public class ProductRepository : IProductRepository
    {
        private readonly CandleShopDbContext _context;

        public ProductRepository(CandleShopDbContext context)
        {
            _context = context;
        }
        public async Task AddProductAsync(Product product)
        {
            await _context.Products.AddAsync(product);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
