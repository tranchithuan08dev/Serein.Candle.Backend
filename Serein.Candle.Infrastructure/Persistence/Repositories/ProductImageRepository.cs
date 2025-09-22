using Serein.Candle.Domain.Entities;
using Serein.Candle.Domain.Interfaces;
using Serein.Candle.Infrastructure.Persistence.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Serein.Candle.Infrastructure.Persistence.Repositories
{
    public class ProductImageRepository : IProductImageRepository
    {
        private readonly CandleShopDbContext _context;

        public ProductImageRepository(CandleShopDbContext context)
        {
            _context = context;
        }
        public async Task AddProductImageAsync(ProductImage productImage)
        {
            await _context.ProductImages.AddAsync(productImage);
        }
    }
}
