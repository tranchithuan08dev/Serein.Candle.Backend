using Microsoft.EntityFrameworkCore;
using Serein.Candle.Domain.Entities;
using Serein.Candle.Domain.Interfaces;
using Serein.Candle.Infrastructure.Persistence.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serein.Candle.Infrastructure.Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly CandleShopDbContext _context;

        public UserRepository(CandleShopDbContext context)
        {
            _context = context;
        }
        public async Task AddUserAsync(User user)
        {
            await _context.Users.AddAsync(user);
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users.AsNoTracking().SingleOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> GetUserByEmailWithRoleAsync(string email)
        {
            return await _context.Users.AsNoTracking().Include(u => u.Role).SingleOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> GetUserWithRoleAsync(int userId)
        {
            return await _context.Users
                             .Include(u => u.Role) 
                             .FirstOrDefaultAsync(u => u.UserId == userId);
        }

        public async Task<bool> IsUserExistsAsync(string email, string phone)
        {
            return await _context.Users.AsNoTracking().AnyAsync(u => u.Email == email || u.Phone == phone);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void UpdateUser(User user)
        {
            _context.Users.Update(user);
        }
    }
}
