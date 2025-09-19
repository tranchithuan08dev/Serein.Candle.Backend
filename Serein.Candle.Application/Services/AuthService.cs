using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serein.Candle.Application.Interfaces;
using Serein.Candle.Domain.DTOs;
using Serein.Candle.Domain.Entities;
using Serein.Candle.Domain.Settings;
using Serein.Candle.Infrastructure.Persistence.Models;
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
        public readonly Serein.Candle.Infrastructure.Persistence.Models.CandleShopDbContext _context;
        public AuthService(CandleShopDbContext context,JwtSettings jwtSettings)
        {
            _context = context;
            _jwtSettings = jwtSettings;
        }

        public async Task<bool> ChangePasswordAsync(ChangePasswordDto changePasswordDto)
        {
            var user = await _context.Users.AsNoTracking().SingleOrDefaultAsync(u => u.Email == changePasswordDto.Email);
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

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return true;
        }

        public Task<LoginResponseDto?> LoginAsync(LoginDto loginDto)
        {
            var user = _context.Users
                               .AsNoTracking()
                               .Include(u => u.Role)
                               .SingleOrDefault(u => u.Email == loginDto.Email);

            if(user == null)
            {
                return Task.FromResult<LoginResponseDto?>(null);
            }
            //Còn Thiếu Mật Khẩu Hash
            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            {
                return Task.FromResult<LoginResponseDto?>(null); 
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Key);

            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.FullName ?? string.Empty),
            new Claim(ClaimTypes.Role, user.Role?.RoleName ?? "Guest")
        };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return Task.FromResult<LoginResponseDto?>(new LoginResponseDto
            {
                Token = tokenHandler.WriteToken(token),
                Email = user.Email,
                FullName = user.FullName,
                RoleName = user.Role.RoleName
            });
        }

        public async Task<bool> RegisterAsync(RegisterDto registerDto)
        {
            var userExists = await _context.Users
                .AsNoTracking()
                .AnyAsync(u => u.Email == registerDto.Email && u.Phone == registerDto.Phone);

            if (userExists)
            {
                return false; 
            }

            string passwordHash = await Task.Run(() => BCrypt.Net.BCrypt.HashPassword(registerDto.Password));


            var customerRole = await _context.RoleTypes.AsNoTracking()
                                                       .SingleOrDefaultAsync(r => r.RoleName == "Customer");
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

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> RegisterStaffAsync(RegisterStaffDto registerStaffDto)
        {
            var userExists = await _context.Users
            .AsNoTracking()
            .AnyAsync(u => u.Email == registerStaffDto.Email && u.Phone == registerStaffDto.Phone);

            if (userExists)
            {
                return false;
            }
            string passwordHash = await Task.Run(() => BCrypt.Net.BCrypt.HashPassword(registerStaffDto.Password));

            var staffRole = await _context.RoleTypes.AsNoTracking().SingleOrDefaultAsync(r => r.RoleName == "Staff");

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
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            var newStaff = new Staff
            {
                UserId = newUser.UserId,
                EmployeeCode = registerStaffDto.EmployeeCode,
                Position = registerStaffDto.Position,
                CreatedAt = DateTime.UtcNow
            };

            _context.Staff.Add(newStaff);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
