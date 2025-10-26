using Serein.Candle.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serein.Candle.Application.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponseDto?> LoginAsync(LoginDto loginDto);
        Task<bool> RegisterAsync(RegisterDto registerDto);
        Task<bool> RegisterStaffAsync(RegisterStaffDto registerStaffDto);
        Task<bool> ChangePasswordAsync(ChangePasswordDto changePasswordDto);
        Task<bool> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto);
        Task<bool> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
        Task<UserDetailDto?> GetCurrentUserDetailsAsync(int userId);
    }
}
