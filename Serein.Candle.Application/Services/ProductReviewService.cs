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
    public class ProductReviewService : IProductReviewService
    {
        private readonly IProductReviewRepository _reviewRepository;
        private readonly IProductRepository _productRepository; 
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public ProductReviewService(
            IProductReviewRepository reviewRepository,
            IProductRepository productRepository,
            IUserRepository userRepository,
            IMapper mapper)
        {
            _reviewRepository = reviewRepository;
            _productRepository = productRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductReviewDto>> GetReviewsByProductIdAsync(int productId)
        {
            var reviews = await _reviewRepository.GetReviewsByProductIdAsync(productId);

            // Ánh xạ thủ công để bao gồm UserName từ User entity
            var dtos = reviews.Select(r => new ProductReviewDto
            {
                ReviewId = r.ReviewId,
                ProductId = r.ProductId,
                UserId = r.UserId,
                Rating = r.Rating,
                Comment = r.Content,
                CreatedAt = r.CreatedAt,
                // Lấy FullName nếu có User, nếu không thì để trống hoặc "Khách"
                UserName = r.User != null ? r.User.FullName : "Khách ẩn danh"
            }).ToList();

            return dtos;
        }

        public async Task<bool> AddProductReviewAsync(int userId, ProductReviewDto dto)
        {
            // 1. Kiểm tra tồn tại sản phẩm
            if (await _productRepository.GetByIdAsync(dto.ProductId) == null)
            {
                return false; // Sản phẩm không tồn tại
            }

            // 2. Tạo entity mới từ DTO
            var newReview = new ProductReview
            {
                ProductId = dto.ProductId,
                UserId = userId, // UserId được lấy từ token
                Rating = dto.Rating,
                Content = dto.Comment,
                CreatedAt = DateTime.UtcNow
            };

            await _reviewRepository.AddReviewAsync(newReview);
            return await _reviewRepository.SaveChangesAsync();
        }
    }
}
