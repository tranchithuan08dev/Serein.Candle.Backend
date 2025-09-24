using Serein.Candle.Domain.DTOs;
using Serein.Candle.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serein.Candle.Application.Interfaces
{
    public interface ICategoryService : IGenericService<Category, CategoryCRUDDto>
    {
      
    }
}
