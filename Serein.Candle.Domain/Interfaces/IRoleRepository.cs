using Serein.Candle.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serein.Candle.Domain.Interfaces
{
    public interface IRoleRepository
    {
        Task<RoleType?> GetRoleByNameAsync(string roleName);
    }
}
