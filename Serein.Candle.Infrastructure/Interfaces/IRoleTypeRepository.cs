using Serein.Candle.Domain.Entities;
using Serein.Candle.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serein.Candle.Infrastructure.Interfaces
{
    public interface IRoleTypeRepository : IGenericRepository<RoleType>
    {
        // Thêm các phương thức đặc thù tại đây nếu cần
    }
}
