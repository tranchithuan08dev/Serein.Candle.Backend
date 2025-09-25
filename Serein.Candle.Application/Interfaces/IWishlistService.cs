using Serein.Candle.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serein.Candle.Application.Interfaces
{
    public interface IWishlistService
    {
        Task<IEnumerable<WishlistDto>> GetUserWishlistAsync(int userId);
        Task<bool> AddToWishlistAsync(int userId, AddWishlistDto dto);
        Task<bool> RemoveFromWishlistAsync(int wishlistId);
    }
}
