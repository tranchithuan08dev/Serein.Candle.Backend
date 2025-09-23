using Serein.Candle.Application.Interfaces;
using Serein.Candle.Domain.DTOs;
using Serein.Candle.Domain.Entities;
using Serein.Candle.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serein.Candle.Application.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly ICartItemRepository _cartItemRepository;
        private readonly IProductRepository _productRepository;

        public CartService(ICartRepository cartRepository, ICartItemRepository cartItemRepository, IProductRepository productRepository)
        {
            _cartRepository = cartRepository;
            _cartItemRepository = cartItemRepository;
            _productRepository = productRepository;
        }

        public async Task<bool> AddProductToCartAsync(int userId, AddCartItemDto cartItemDto)
        {
            // 1. Tìm giỏ hàng hiện có của người dùng
            var existingCart = await _cartRepository.GetCartByUserIdAsync(userId);

            // 2. Nếu giỏ hàng không tồn tại, tạo mới
            if (existingCart == null)
            {
                existingCart = new Cart
                {
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow
                };
                await _cartRepository.AddCartAsync(existingCart);
                await _cartRepository.SaveChangesAsync();
            }

            // 3. Lấy thông tin sản phẩm để có giá
            var product = await _productRepository.GetByIdAsync(cartItemDto.ProductId);
            if (product == null)
            {
                return false;
            }

            // 4. Tìm sản phẩm trong giỏ hàng
            var existingCartItem = await _cartItemRepository.GetCartItemAsync(existingCart.CartId, cartItemDto.ProductId);

            if (existingCartItem != null)
            {
                // 5. Nếu sản phẩm đã tồn tại, cập nhật số lượng
                existingCartItem.Quantity += cartItemDto.Quantity;
            }
            else
            {
                // 6. Nếu sản phẩm chưa tồn tại, thêm mới
                var newCartItem = new CartItem
                {
                    CartId = existingCart.CartId,
                    ProductId = cartItemDto.ProductId,
                    Quantity = cartItemDto.Quantity,
                    PriceAtAdd = product.Price
                };
                await _cartItemRepository.AddCartItemAsync(newCartItem);
            }

            return await _cartItemRepository.SaveChangesAsync();
        }
    }
}
