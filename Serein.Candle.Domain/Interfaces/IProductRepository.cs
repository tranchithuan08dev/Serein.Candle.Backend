using Serein.Candle.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serein.Candle.Domain.Interfaces
{
    public interface IProductRepository
    {
        Task AddProductAsync(Product product);
        Task<Product?> GetProductDetailAsync(int productId);
        Task<Product?> GetByIdAsync(int id);
        Task UpdateProductAsync(Product product);
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task<int> SaveChangesAsync();
    }
}
