using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serein.Candle.Application.Interfaces
{
    public interface IGenericService<T, TDto> where T : class
    {
        Task<IEnumerable<TDto>> GetAllAsync();
        Task<TDto?> GetByIdAsync(int id);
        Task<bool> AddAsync(TDto dto);
        Task<bool> UpdateAsync(int id, TDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
