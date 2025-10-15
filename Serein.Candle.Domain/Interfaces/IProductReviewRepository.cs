using Serein.Candle.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serein.Candle.Domain.Interfaces
{
    public interface IProductReviewRepository
    {
       
        Task<IEnumerable<ProductReview>> GetReviewsByProductIdAsync(int productId);

        Task AddReviewAsync(ProductReview review);

        Task<bool> SaveChangesAsync();
    }
}
