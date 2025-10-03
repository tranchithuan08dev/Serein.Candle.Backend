using Serein.Candle.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serein.Candle.Application.Interfaces
{
    public interface IOrderService
    {
        // Lấy danh sách Order của User
        Task<IEnumerable<OrderDetailDto>> GetUserOrdersAsync(int userId);

        // Lấy chi tiết Order
        Task<OrderDetailDto?> GetOrderDetailsAsync(int orderId, int userId);

        // Tạo đơn hàng mới
        Task<(bool Success, string Message, string OrderCode)> CreateOrderAsync(int userId, OrderCreationDto dto);
    }
}
