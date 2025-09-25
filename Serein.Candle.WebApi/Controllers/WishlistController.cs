using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serein.Candle.Application.Interfaces;
using Serein.Candle.Domain.DTOs;
using System.Security.Claims;

namespace Serein.Candle.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class WishlistController : Controller
    {
        private readonly IWishlistService _wishlistService;

        public WishlistController(IWishlistService wishlistService)
        {
            _wishlistService = wishlistService;
        }

        // Lấy UserId từ Claims
        private int GetUserId()
        {
            // Thay thế bằng logic lấy UserId từ HttpContext.User.Claims chính xác của bạn
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.Parse(userIdClaim ?? "0");
        }

        // GET: api/Wishlist
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<WishlistDto>), 200)]
        public async Task<IActionResult> GetWishlist()
        {
            var userId = GetUserId();
            var wishlist = await _wishlistService.GetUserWishlistAsync(userId);
            return Ok(wishlist);
        }

        // POST: api/Wishlist
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> AddToWishlist([FromBody] AddWishlistDto dto)
        {
            var userId = GetUserId();
            var success = await _wishlistService.AddToWishlistAsync(userId, dto);

            if (success)
            {
                return Ok(new { Message = "Sản phẩm đã được thêm vào Wishlist." });
            }

            return BadRequest(new { Message = "Không thể thêm sản phẩm vào Wishlist." });
        }

        // DELETE: api/Wishlist/{wishlistId}
        [HttpDelete("{wishlistId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> RemoveFromWishlist(int wishlistId)
        {
            var success = await _wishlistService.RemoveFromWishlistAsync(wishlistId);

            if (success)
            {
                return NoContent(); // 204 No Content
            }

            return NotFound(new { Message = "Mục Wishlist không tồn tại hoặc không thể xóa." });
        }
    }
}
