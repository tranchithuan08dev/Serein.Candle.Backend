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

        public async Task<CartDetailDto?> GetCartByUserIdAsync(int userId)
        {
            var cart = await _cartRepository.GetCartByUserIdAsync(userId);
            if (cart == null)
            {
                return null;
            }

            var totalAmount = cart.CartItems.Sum(item => item.Quantity * item.PriceAtAdd);
            var cartDto = new CartDetailDto
            {
                CartId = cart.CartId,
                UserId = cart.UserId,
                TotalAmount = totalAmount,
                Items = new List<CartItemDto>()
            };

            // Vòng lặp để lấy chi tiết từng sản phẩm
            foreach (var item in cart.CartItems)
            {
                var productDetail = await _productRepository.GetProductDetailAsync(item.ProductId);

                if (productDetail != null)
                {
                    var cartItemDto = new CartItemDto
                    {
                        ProductId = item.ProductId,
                        ProductName = productDetail.Name,
                        ProductSku = productDetail.Sku,
                        Quantity = item.Quantity,
                        PriceAtAdd = item.PriceAtAdd,
                        ImageUrl = productDetail.ProductImages.FirstOrDefault()?.ImageUrl ?? string.Empty,
                        // Bạn có thể thêm các thông tin khác từ productDetail vào đây nếu cần
                    };
                    cartDto.Items.Add(cartItemDto);
                }
            }

            return cartDto;
        }

        public async Task<bool> RemoveProductFromCartAsync(int userId, int productId)
        {
            // 1. Tìm giỏ hàng của người dùng
            var existingCart = await _cartRepository.GetCartByUserIdAsync(userId);
            if (existingCart == null)
            {
                return false;
            }

            // 2. Tìm mặt hàng cần xóa trong giỏ hàng
            var existingCartItem = await _cartItemRepository.GetCartItemAsync(existingCart.CartId, productId);
            if (existingCartItem == null)
            {
                return false;
            }

            // 3. Xóa mặt hàng
            _cartItemRepository.RemoveCartItem(existingCartItem);

            // 4. Lưu thay đổi vào database
            return await _cartItemRepository.SaveChangesAsync();
        }
    }
}
