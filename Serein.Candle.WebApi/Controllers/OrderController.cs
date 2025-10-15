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
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        // Phương thức Helper lấy User ID (giả định)
        private int GetUserId()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(userIdClaim, out int userId))
            {
                return userId;
            }
            return 0;
        }

        // GET: api/Order
        // Lấy danh sách đơn hàng của người dùng đã đăng nhập
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<OrderDetailDto>), 200)]
        public async Task<IActionResult> GetUserOrders()
        {
            var userId = GetUserId();
            if (userId == 0) return Unauthorized();

            var orders = await _orderService.GetUserOrdersAsync(userId);
            return Ok(orders);
        }

        // GET: api/Order/{orderId}
        // Lấy chi tiết một đơn hàng
        [HttpGet("{orderId}")]
        [ProducesResponseType(typeof(OrderDetailDto), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetOrderDetails(int orderId)
        {
            var userId = GetUserId();
            if (userId == 0) return Unauthorized();

            // Cần truyền cả userId để đảm bảo người dùng chỉ xem được đơn hàng của mình
            var order = await _orderService.GetOrderDetailsAsync(orderId, userId);

            if (order == null)
            {
                return NotFound(new { Message = "Đơn hàng không tồn tại hoặc bạn không có quyền truy cập." });
            }

            return Ok(order);
        }

        // POST: api/Order
        // Tạo đơn hàng mới
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateOrder([FromBody] OrderCreationDto dto)
        {
            var userId = GetUserId();
            if (userId == 0) return Unauthorized();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _orderService.CreateOrderAsync(userId, dto);

            if (result.Success)
            {
                return StatusCode(201, new { Message = result.Message, OrderCode = result.OrderCode });
            }

            return BadRequest(new { Message = result.Message });
        }


        [HttpPut("{orderId}/status")]
        // TODO: Bạn cần cấu hình Policy hoặc Roles trong hệ thống để sử dụng dòng này
        // [Authorize(Roles = "Staff, Admin")] 
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, [FromBody] UpdateOrderStatusDto dto)
        {
       
            var result = await _orderService.UpdateOrderStatusAsync(orderId, dto);

            if (result.Success)
            {
                return Ok(new { Message = result.Message });
            }

            return BadRequest(new { Message = result.Message });
        }
    }
}
