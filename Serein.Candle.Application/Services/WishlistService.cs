using AutoMapper;
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
    public class WishlistService : IWishlistService
    {
        private readonly IWishlistRepository _wishlistRepository;
        private readonly IMapper _mapper;
        private readonly IProductRepository _productRepository; // Cần dùng để kiểm tra sự tồn tại của Product

        public WishlistService(IWishlistRepository wishlistRepository, IMapper mapper, IProductRepository productRepository)
        {
            _wishlistRepository = wishlistRepository;
            _mapper = mapper;
            _productRepository = productRepository;
        }

        public async Task<IEnumerable<WishlistDto>> GetUserWishlistAsync(int userId)
        {
            var wishlistItems = await _wishlistRepository.GetWishlistByUserIdAsync(userId);

            // Ánh xạ sang DTO thủ công vì cần lấy dữ liệu lồng từ Product/ProductImage
            var dtos = wishlistItems.Select(item => new WishlistDto
            {
                WishlistId = item.WishlistId,
                ProductId = item.ProductId,
                ProductName = item.Product.Name,
                ProductSku = item.Product.Sku,
                Price = item.Product.Price,
                ImageUrl = item.Product.ProductImages.FirstOrDefault()?.ImageUrl ?? string.Empty,
                CreatedAt = item.CreatedAt
            }).ToList();

            return dtos;
        }

        public async Task<bool> AddToWishlistAsync(int userId, AddWishlistDto dto)
        {
            // 1. Kiểm tra sản phẩm có tồn tại không
            if (await _productRepository.GetByIdAsync(dto.ProductId) == null)
            {
                return false; // Product không tồn tại
            }

            // 2. Kiểm tra đã có trong Wishlist chưa (dựa trên Unique Constraint)
            if (await _wishlistRepository.GetExistingItemAsync(userId, dto.ProductId) != null)
            {
                return true; // Đã tồn tại, coi như thành công
            }

            // 3. Thêm mới
            var newItem = new Wishlist
            {
                UserId = userId,
                ProductId = dto.ProductId,
                CreatedAt = DateTime.UtcNow
            };

            await _wishlistRepository.AddAsync(newItem);
            return await _wishlistRepository.SaveChangesAsync();
        }

        public async Task<bool> RemoveFromWishlistAsync(int wishlistId)
        {
            // Sử dụng phương thức DeleteByIdAsync để xóa
            var success = await _wishlistRepository.DeleteByIdAsync(wishlistId);
            if (success)
            {
                return await _wishlistRepository.SaveChangesAsync();
            }
            return false;
        }
    }
}
