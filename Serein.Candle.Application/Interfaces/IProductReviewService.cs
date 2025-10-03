using Serein.Candle.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serein.Candle.Application.Interfaces
{
    public interface IProductReviewService
    {
        Task<IEnumerable<ProductReviewDto>> GetReviewsByProductIdAsync(int productId);
        Task<bool> AddProductReviewAsync(int userId, ProductReviewDto dto);
    }
}
