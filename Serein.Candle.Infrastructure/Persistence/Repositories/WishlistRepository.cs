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
    public class WishlistRepository : IWishlistRepository
    {
        private readonly CandleShopDbContext _context;

        public WishlistRepository(CandleShopDbContext context)
        {
            _context = context;
        }

        public async Task<List<Wishlist>> GetWishlistByUserIdAsync(int userId)
        {
            return await _context.Wishlists
                                 .Where(w => w.UserId == userId)
                                 .Include(w => w.Product)
                                    .ThenInclude(p => p.ProductImages.Where(pi => pi.IsPrimary).Take(1))
                                 .ToListAsync();
        }

        public async Task<Wishlist?> GetExistingItemAsync(int userId, int productId)
        {
            return await _context.Wishlists
                                 .FirstOrDefaultAsync(w => w.UserId == userId && w.ProductId == productId);
        }

        public async Task AddAsync(Wishlist wishlistItem)
        {
            await _context.Wishlists.AddAsync(wishlistItem);
        }

        public Task RemoveAsync(Wishlist wishlistItem)
        {
            _context.Wishlists.Remove(wishlistItem);
            return Task.CompletedTask;
        }

        public async Task<bool> DeleteByIdAsync(int wishlistId)
        {
            var item = await _context.Wishlists.FindAsync(wishlistId);
            if (item == null) return false;

            _context.Wishlists.Remove(item);
            return true;
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
