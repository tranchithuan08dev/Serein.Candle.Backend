using Serein.Candle.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serein.Candle.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetUserByEmailAsync(string email);
        Task<User?> GetUserByEmailWithRoleAsync(string email);
        Task<bool> IsUserExistsAsync(string email, string phone);
        Task AddUserAsync(User user);
        void UpdateUser(User user);
        Task<User?> GetUserWithRoleAsync(int userId);
        Task<int> SaveChangesAsync();

    }
}
