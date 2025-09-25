using Serein.Candle.Domain.DTOs;
using Serein.Candle.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serein.Candle.Application.Interfaces
{
    public interface IRoleTypeService : IGenericService<RoleType, RoleTypeCRUDDto>
    {
        // Thêm các phương thức đặc thù tại đây nếu cần
    }
}
