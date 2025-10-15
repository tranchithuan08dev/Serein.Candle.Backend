using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serein.Candle.Application.Interfaces;
using Serein.Candle.Domain.DTOs;
using System.Security.Claims;

namespace Serein.Candle.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductReviewController : ControllerBase
    {
        private readonly IProductReviewService _reviewService;

        public ProductReviewController(IProductReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        // Sử dụng lại phương thức GetUserId() đã được cải tiến
        private int GetUserId()
        {
            // Thay thế bằng logic lấy UserId từ HttpContext.User.Claims chính xác của bạn
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.Parse(userIdClaim ?? "0");
        }

        // GET: api/ProductReview?productId=5
        // Lấy tất cả đánh giá cho một sản phẩm
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ProductReviewDto>), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetReviews([FromQuery] int productId)
        {
            if (productId <= 0)
            {
                return BadRequest(new { Message = "Yêu cầu phải cung cấp ProductId hợp lệ." });
            }

            var reviews = await _reviewService.GetReviewsByProductIdAsync(productId);
            return Ok(reviews);
        }

        // POST: api/ProductReview
        // Thêm đánh giá mới (yêu cầu đăng nhập)
        [HttpPost]
        [Authorize] // Bắt buộc đăng nhập để đánh giá
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> PostReview([FromBody] ProductReviewDto dto)
        {
            var userId = GetUserId();
            if (userId == 0) return Unauthorized();

            if (dto.Rating < 1 || dto.Rating > 5)
            {
                return BadRequest(new { Message = "Rating phải nằm trong khoảng từ 1 đến 5." });
            }

            var success = await _reviewService.AddProductReviewAsync(userId, dto);

            if (success)
            {
                return StatusCode(201, new { Message = "Đánh giá của bạn đã được gửi thành công." });
            }

            return BadRequest(new { Message = "Không thể thêm đánh giá. Vui lòng kiểm tra lại ProductId." });
        }
    }
}
