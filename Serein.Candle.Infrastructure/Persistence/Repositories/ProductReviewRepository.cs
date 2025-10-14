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
    public class ProductReviewRepository : IProductReviewRepository
    {
        private readonly CandleShopDbContext _context;

        public ProductReviewRepository(CandleShopDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProductReview>> GetReviewsByProductIdAsync(int productId)
        {
            return await _context.ProductReviews
                                 .Where(r => r.ProductId == productId)
                                 .Include(r => r.User)
                                 .OrderByDescending(r => r.CreatedAt)
                                 .ToListAsync();
        }

        public async Task AddReviewAsync(ProductReview review)
        {
            await _context.ProductReviews.AddAsync(review);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
