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
    public class RoleRepository : IRoleRepository
    {
        private readonly CandleShopDbContext _context;

        public RoleRepository(CandleShopDbContext context)
        {
            _context = context;
        }
        public async Task<RoleType?> GetRoleByNameAsync(string roleName)
        {
            return await _context.RoleTypes.AsNoTracking().SingleOrDefaultAsync(r => r.RoleName == roleName);
        }
    }
}
