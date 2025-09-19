using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serein.Candle.Application.Interfaces;
using Serein.Candle.Domain.DTOs;
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
    }
}
