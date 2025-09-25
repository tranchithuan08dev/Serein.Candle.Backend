using Serein.Candle.Domain.Entities;
using Serein.Candle.Infrastructure.Interfaces;
using Serein.Candle.Infrastructure.Persistence.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serein.Candle.Infrastructure.Persistence.Repositories
{
    public class RoleTypeRepository : GenericRepository<RoleType>, IRoleTypeRepository
    {
        public RoleTypeRepository(CandleShopDbContext context) : base(context)
        {
        }
    }
}
