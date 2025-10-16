using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using Serein.Candle.Application.Interfaces;
using Serein.Candle.Domain.DTOs;
using Serein.Candle.Domain.Entities;
using Serein.Candle.Domain.Interfaces;
using Serein.Candle.Domain.Settings;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Serein.Candle.Application.Services
{
    public class AuthService : IAuthService
    {
        public readonly JwtSettings _jwtSettings;
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IStaffRepository _staffRepository;
        private readonly IEmailService _emailService;
        private readonly IMemoryCache _memoryCache;
        public AuthService(IUserRepository userRepository, IRoleRepository roleRepository, IStaffRepository staffRepository, IEmailService emailService, IMemoryCache memoryCache, JwtSettings jwtSettings)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _staffRepository = staffRepository;
            _emailService = emailService;
            _memoryCache = memoryCache;
            _jwtSettings = jwtSettings;
        }

        public async Task<bool> ChangePasswordAsync(ChangePasswordDto changePasswordDto)
        {
            var user = await _userRepository.GetUserByEmailAsync(changePasswordDto.Email);
            if (user == null)
            {
                return false;
            }
            if (!BCrypt.Net.BCrypt.Verify(changePasswordDto.OldPassword, user.PasswordHash))
            {
                return false;
            }
            string newPasswordHash = BCrypt.Net.BCrypt.HashPassword(changePasswordDto.NewPassword);
            user.PasswordHash = newPasswordHash;
            user.UpdatedAt = DateTime.UtcNow;

            _userRepository.UpdateUser(user);
            await _userRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto)
        {
            var user = await _userRepository.GetUserByEmailAsync(forgotPasswordDto.Email);
            if (user == null)
            {
                return false;
            }
            var otp = new Random().Next(100000, 999999).ToString();
            _memoryCache.Set(forgotPasswordDto.Email, otp, TimeSpan.FromMinutes(5));
            var subject = "Mã OTP để đặt lại mật khẩu của bạn";
            var body = $"Mã OTP của bạn là: <b>{otp}</b>. Mã này sẽ hết hạn sau 5 phút.";
            await _emailService.SendEmailAsync(forgotPasswordDto.Email, subject, body);

            return true;
        }

        public async Task<LoginResponseDto?> LoginAsync(LoginDto loginDto)
        {
            var user = await _userRepository.GetUserByEmailWithRoleAsync(loginDto.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            {
                return null;
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Key);

            var claims = new List<Claim>
        {
           new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.FullName ?? string.Empty),
            new Claim(ClaimTypes.Role, user.Role?.RoleName ?? "Guest")
        };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes > 0 ? _jwtSettings.DurationInMinutes : 60),
                Issuer = _jwtSettings.Issuer,        
                Audience = _jwtSettings.Audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return new LoginResponseDto
            {
                Token = tokenHandler.WriteToken(token),
                Email = user.Email,
                FullName = user.FullName,
                RoleName = user.Role.RoleName
            };
        }

        public async Task<bool> RegisterAsync(RegisterDto registerDto)
        {
            var userExists = await _userRepository.IsUserExistsAsync(registerDto.Email, registerDto.Phone);
            if (userExists)
            {
                return false;
            }

            string passwordHash = await Task.Run(() => BCrypt.Net.BCrypt.HashPassword(registerDto.Password));

            var customerRole = await _roleRepository.GetRoleByNameAsync("Customer");
            if (customerRole == null)
            {
                throw new Exception("Role 'Customer' not found.");
            }

            var newUser = new User
            {
                Email = registerDto.Email,
                Phone = registerDto.Phone,
                FullName = registerDto.FullName,
                PasswordHash = passwordHash,
                IsGuest = false,
                Dob = DateOnly.FromDateTime(registerDto.DateOfBirth),
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                RoleId = customerRole.RoleId
            };

            await _userRepository.AddUserAsync(newUser);
            await _userRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> RegisterStaffAsync(RegisterStaffDto registerStaffDto)
        {
            var userExists = await _userRepository.IsUserExistsAsync(registerStaffDto.Email, registerStaffDto.Phone);
            if (userExists)
            {
                return false;
            }
            string passwordHash = await Task.Run(() => BCrypt.Net.BCrypt.HashPassword(registerStaffDto.Password));

            var staffRole = await _roleRepository.GetRoleByNameAsync("Staff");
            if (staffRole == null)
            {
                throw new Exception("Role 'Staff' not found.");
            }

            var newUser = new User
            {
                Email = registerStaffDto.Email,
                Phone = registerStaffDto.Phone,
                FullName = registerStaffDto.FullName,
                PasswordHash = passwordHash,
                RoleId = staffRole.RoleId,
                Dob = DateOnly.FromDateTime(registerStaffDto.DateOfBirth),
                IsGuest = false,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            await _userRepository.AddUserAsync(newUser);
            await _userRepository.SaveChangesAsync();

            var newStaff = new Staff
            {
                UserId = newUser.UserId,
                EmployeeCode = registerStaffDto.EmployeeCode,
                Position = registerStaffDto.Position,
                CreatedAt = DateTime.UtcNow
            };

            await _staffRepository.AddStaffAsync(newStaff);
            await _userRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {

            var user = await _userRepository.GetUserByEmailAsync(resetPasswordDto.Email);
            if (user == null)
            {
                return false;
            }

            if (!_memoryCache.TryGetValue(resetPasswordDto.Email, out string? storedOtp) || storedOtp != resetPasswordDto.Otp)
            {
                return false;
            }

            string newPasswordHash = BCrypt.Net.BCrypt.HashPassword(resetPasswordDto.NewPassword);
            user.PasswordHash = newPasswordHash;
            user.UpdatedAt = DateTime.UtcNow;

            _userRepository.UpdateUser(user);
            await _userRepository.SaveChangesAsync();

            _memoryCache.Remove(resetPasswordDto.Email);

            return true;
        }
    }
}
