using Microsoft.AspNetCore.Mvc;
using Serein.Candle.Application.Interfaces;
using Serein.Candle.Domain.DTOs;
using Serein.Candle.WebApi.Responses;
using System.Security.Claims;

namespace Serein.Candle.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        private int GetUserId()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(userIdClaim, out int userId))
            {
                return userId;
            }
            return 0;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var response = await _authService.LoginAsync(loginDto);
            if (response == null)
            {
                return Unauthorized(new ApiResponse<object> { 
                   Success = false,
                    Message = "Email hoặc mật khẩu không đúng.",
                });
            }

            return Ok(new ApiResponse<object>(
                true,
                "Đăng nhập thành công.",
                response
            ));
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                var result = await _authService.RegisterAsync(registerDto);

                if (!result)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Email hoặc số điện thoại đã tồn tại."
                    });
                }

                return CreatedAtAction(nameof(Register), new ApiResponse<object>
                {
                    Success = true,
                    Message = "Đăng ký thành công."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Đã xảy ra lỗi trong quá trình xử lý.",
                    Data = ex.Message 
                });
            }
        }

        [HttpPost("register/staff")]
        public async Task<IActionResult> RegisterStaff([FromBody] RegisterStaffDto registerStaffDto)
        {

            try
            {
                var result = await _authService.RegisterStaffAsync(registerStaffDto);
                if (!result)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Email hoặc số điện thoại đã tồn tại."
                    });
                }

                return CreatedAtAction(nameof(Register), new ApiResponse<object>
                {
                    Success = true,
                    Message = "Đăng ký thành công."
                });
            }
            catch(Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Đã xảy ra lỗi trong quá trình xử lý.",
                    Data = ex.Message
                });
            }
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            var result = await _authService.ChangePasswordAsync(changePasswordDto);

            if (!result)
            {
                return BadRequest(new ApiResponse<object>(
                    false,
                    "Email hoặc mật khẩu cũ không đúng.",
                    null
                ));
            }

            return Ok(new ApiResponse<object>(
                true,
                "Mật khẩu đã được đổi thành công.",
                null
            ));
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
        {
            var result = await _authService.ForgotPasswordAsync(forgotPasswordDto);
            if (!result)
            {
                return BadRequest(new { message = "Email không tồn tại." });
            }
            return Ok(new { message = "Mã OTP đã được gửi đến email của bạn." });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
        {
            var result = await _authService.ResetPasswordAsync(resetPasswordDto);
            if (!result)
            {
                return BadRequest(new { message = "Mã OTP không hợp lệ hoặc đã hết hạn." });
            }
            return Ok(new { message = "Mật khẩu đã được đặt lại thành công." });
        }


        [HttpGet("me")]
        [ProducesResponseType(typeof(UserDetailDto), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetCurrentUserProfile()
        {
            // 1. Trích xuất User ID từ Token (Claims)
            var userId = GetUserId();

            // 2. Kiểm tra ID
            if (userId == 0)
            {
                // Lỗi này hiếm khi xảy ra nếu [Authorize] đã thành công
                return Unauthorized(new { Message = "Không tìm thấy thông tin người dùng trong token." });
            }

            // 3. Gọi Service
            var userDetails = await _authService.GetCurrentUserDetailsAsync(userId);

            if (userDetails == null)
            {
                return NotFound(new { Message = "Hồ sơ người dùng không tồn tại." });
            }

            // 4. Trả về DTO
            return Ok(userDetails);
        }

    }
}
