using Serein.Candle.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serein.Candle.Domain.Interfaces
{
    public interface IWishlistRepository
    { 
        Task<List<Wishlist>> GetWishlistByUserIdAsync(int userId);

        Task<Wishlist?> GetExistingItemAsync(int userId, int productId);

        Task AddAsync(Wishlist wishlistItem);
        Task RemoveAsync(Wishlist wishlistItem);
        Task<bool> DeleteByIdAsync(int wishlistId);
        Task<bool> SaveChangesAsync();
    }
}
