using Microsoft.EntityFrameworkCore;
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

        public IQueryable<Product> GetAllProducts()
        {
            return _context.Products
             .Include(p => p.Category)
             .Include(p => p.ProductImages)
             .Include(p => p.ProductAttributeValues)
             .ThenInclude(pav => pav.Attribute)
             .AsNoTracking();
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _context.Products
                        .Include(p => p.Category)
                        .Include(p => p.ProductImages)
                        .Include(p => p.ProductAttributeValues)
                        .ThenInclude(pav => pav.Attribute)
                        .AsNoTracking() 
                        .ToListAsync();
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _context.Products.FindAsync(id);
        }

        public async Task<Product?> GetProductDetailAsync(int productId)
        {
            return await _context.Products
             .Include(p => p.Category)
             .Include(p => p.ProductAttributeValues)
             .ThenInclude(pav => pav.Attribute)
             .Include(p => p.ProductImages)
             .SingleOrDefaultAsync(p => p.ProductId == productId);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public Task UpdateProductAsync(Product product)
        {
            _context.Products.Update(product);
            return Task.CompletedTask;
        }
    }
}
