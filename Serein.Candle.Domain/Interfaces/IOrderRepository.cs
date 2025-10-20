using Serein.Candle.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serein.Candle.Domain.Interfaces
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetOrdersByUserIdAsync(int userId);

        // Lấy chi tiết đơn hàng, bao gồm OrderItems, Status, PaymentMethod, Address
        Task<Order?> GetOrderDetailsByIdAsync(int orderId);

        Task<IEnumerable<Order>> GetAllOrdersAsync();
        // Thêm Address và Order
        Task AddAddressAsync(CustomerAddress address);
        Task AddOrderAsync(Order order);

        // Tìm kiếm thông tin cần thiết để tính toán
        Task<Product?> GetProductByIdAsync(int productId);
        Task<Voucher?> GetVoucherByCodeAsync(string code);

        Task<Order?> GetOrderByIdAsync(int orderId);

        Task<bool> IsStatusIdValidAsync(int statusId);

        Task<bool> SaveChangesAsync();
    }
}
