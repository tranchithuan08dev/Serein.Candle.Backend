using Serein.Candle.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serein.Candle.Application.Interfaces
{
    public interface ICartService
    {
        Task<bool> AddProductToCartAsync(int userId, AddCartItemDto cartItemDto);
    }
}
