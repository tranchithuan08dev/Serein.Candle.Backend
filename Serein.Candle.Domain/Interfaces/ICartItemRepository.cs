using Serein.Candle.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serein.Candle.Domain.Interfaces
{
    public interface ICartItemRepository
    {
        Task AddCartItemAsync(CartItem cartItem);
        Task<CartItem?> GetCartItemAsync(int cartId, int productId);
        void RemoveCartItem(CartItem cartItem);
        Task<Cart?> GetCartByUserIdAsync(int userId);
        Task<bool> SaveChangesAsync();
    }
}
