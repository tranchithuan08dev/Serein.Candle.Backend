using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serein.Candle.Application.Interfaces;
using Serein.Candle.Domain.DTOs;
using System.Security.Claims;

namespace Serein.Candle.WebApi.Controllers
{
    [ApiController]
    [Route("api/carts")]
    public class CartController : Controller
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }
        [Authorize(Roles = "Customer")]
        [HttpPost("items")]
        public async Task<IActionResult> AddProductToCart([FromBody] AddCartItemDto cartItemDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return Unauthorized("User is not authenticated or user ID is invalid.");
            }

            var result = await _cartService.AddProductToCartAsync(userId, cartItemDto);
            if (result)
            {
                return Ok("Product added to cart successfully.");
            }

            return BadRequest("Failed to add product to cart. Product might not exist.");
        }
    }
}
