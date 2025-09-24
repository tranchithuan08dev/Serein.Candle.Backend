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


        [HttpDelete("items/{productId}")]
        public async Task<IActionResult> RemoveProductFromCart(int productId)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return Unauthorized("User is not authenticated or user ID is invalid.");
            }

            var result = await _cartService.RemoveProductFromCartAsync(userId, productId);
            if (result)
            {
                return Ok("Product removed from cart successfully.");
            }

            return NotFound("Product not found in cart or failed to remove.");
        }

        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return Unauthorized("User is not authenticated or user ID is invalid.");
            }

            var cart = await _cartService.GetCartByUserIdAsync(userId);
            if (cart == null)
            {
                return NotFound("Cart not found for this user.");
            }

            return Ok(cart);
        }
    }
}
