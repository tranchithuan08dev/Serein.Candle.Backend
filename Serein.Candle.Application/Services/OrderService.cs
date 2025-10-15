using AutoMapper;
using Serein.Candle.Application.Interfaces;
using Serein.Candle.Domain.DTOs;
using Serein.Candle.Domain.Entities;
using Serein.Candle.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serein.Candle.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;

        public OrderService(IOrderRepository orderRepository, IMapper mapper)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<OrderDetailDto>> GetUserOrdersAsync(int userId)
        {
            var orders = await _orderRepository.GetOrdersByUserIdAsync(userId);

            // Ánh xạ thủ công hoặc dùng AutoMapper (cần setup profile)
            return orders.Select(o => new OrderDetailDto
            {
                OrderId = o.OrderId,
                OrderCode = o.OrderCode,
                StatusName = o.Status.StatusName,
                TotalAmount = o.TotalAmount,
                CreatedAt = o.CreatedAt
                // Các trường khác có thể được lấy qua một hàm riêng nếu cần
            }).ToList();
        }

        public async Task<OrderDetailDto?> GetOrderDetailsAsync(int orderId, int userId)
        {
            var order = await _orderRepository.GetOrderDetailsByIdAsync(orderId);

            // Kiểm tra quyền sở hữu và sự tồn tại
            if (order == null || order.UserId != userId)
            {
                return null;
            }

            // Ánh xạ Order -> OrderDetailDto
            var dto = new OrderDetailDto
            {
                OrderId = order.OrderId,
                OrderCode = order.OrderCode,
                StatusName = order.Status.StatusName,
                PaymentMethodName = order.PaymentMethod.MethodName,
                TotalAmount = order.TotalAmount,
                DiscountAmount = order.DiscountAmount,
                ShippingFee = order.ShippingFee,
                CreatedAt = order.CreatedAt,

                // Thông tin địa chỉ
                RecipientFullName = order.ShippingAddress.FullName,
                RecipientPhone = order.ShippingAddress.Phone,
                RecipientAddress = $"{order.ShippingAddress.AddressLine}, {order.ShippingAddress.Ward}, {order.ShippingAddress.District}, {order.ShippingAddress.City}",

                // Chi tiết sản phẩm
                Items = order.OrderItems.Select(oi => new OrderItemDetailDto
                {
                    ProductId = oi.ProductId,
                    ProductName = oi.Product.Name,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice,
                    TotalPrice = 0
                }).ToList()
            };

            return dto;
        }


        public async Task<(bool Success, string Message, string OrderCode)> CreateOrderAsync(int userId, OrderCreationDto dto)
        {
            decimal subtotal = 0;
            var orderItems = new List<OrderItem>();

            // 1. Kiểm tra Sản phẩm và Tính Subtotal
            foreach (var item in dto.Items)
            {
                var product = await _orderRepository.GetProductByIdAsync(item.ProductId);

                if (product == null || !product.IsActive)
                {
                    return (false, $"Sản phẩm ID {item.ProductId} không tồn tại hoặc đã ngừng bán.", string.Empty);
                }
                // (TODO: Thêm logic kiểm tra tồn kho tại đây)

                orderItems.Add(new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = product.Price, // Giá tại thời điểm đặt hàng
                });
                subtotal += item.Quantity * product.Price;
            }

            // 2. Xử lý Voucher (ví dụ đơn giản)
            decimal discountAmount = 0;
            if (!string.IsNullOrWhiteSpace(dto.VoucherCode))
            {
                var voucher = await _orderRepository.GetVoucherByCodeAsync(dto.VoucherCode);
                // (TODO: Thêm logic kiểm tra MinOrderAmount, EndDate, MaxUses/UsedCount)
                if (voucher?.DiscountPercent > 0)
                {
                    discountAmount = subtotal * (voucher.DiscountPercent.Value / 100m);
                }
                else if (voucher?.DiscountAmount > 0)
                {
                    discountAmount = voucher.DiscountAmount.Value;
                }
            }

            // 3. Tạo CustomerAddress
            var address = new CustomerAddress
            {
                UserId = userId,
                FullName = dto.FullName,
                Phone = dto.Phone,
                AddressLine = dto.AddressLine,
                City = dto.City,
                District = dto.District,
                Ward = dto.Ward,
                CreatedAt = DateTime.UtcNow
            };
            await _orderRepository.AddAddressAsync(address);

            // 4. Tạo Order
            // Mã đơn hàng (TODO: Tạo mã đơn hàng duy nhất, ví dụ: "SREIN-20250930-12345")
            string orderCode = $"SREIN-{DateTime.Now.ToString("yyyyMMddHHmmss")}";

            decimal shippingFee = 0; // Giả định phí vận chuyển bằng 0
            decimal totalAmount = subtotal - discountAmount + shippingFee;

            var order = new Order
            {
                UserId = userId,
                OrderCode = orderCode,
                StatusId = 1, // 'Pending'
                ShippingAddress = address, // EF Core sẽ tự động liên kết sau khi SaveChanges
                TotalAmount = totalAmount,
                DiscountAmount = discountAmount,
                ShippingFee = shippingFee,
                PaymentMethodId = dto.PaymentMethodId,
                OrderItems = orderItems,
                Note = dto.Note,
                CreatedAt = DateTime.UtcNow
            };
            await _orderRepository.AddOrderAsync(order);

            // 5. Lưu toàn bộ Transaction
            if (await _orderRepository.SaveChangesAsync())
            {
                return (true, "Đặt hàng thành công. Chờ xác nhận thanh toán.", orderCode);
            }

            return (false, "Lỗi hệ thống khi lưu đơn hàng.", string.Empty);
        }

        public async Task<(bool Success, string Message)> UpdateOrderStatusAsync(int orderId, UpdateOrderStatusDto dto)
        {
            // I. Kiểm tra tính hợp lệ của dữ liệu

            // 1. Kiểm tra Order có tồn tại không
            var order = await _orderRepository.GetOrderByIdAsync(orderId);
            if (order == null)
            {
                return (false, "Mã đơn hàng không tồn tại.");
            }

            // 2. Kiểm tra StatusId mới có hợp lệ không
            if (!await _orderRepository.IsStatusIdValidAsync(dto.StatusId)) // <-- DÙNG HÀM KIỂM TRA ID
            {
                return (false, $"Status ID {dto.StatusId} không hợp lệ. Vui lòng kiểm tra bảng OrderStatus.");
            }

            // 3. Kiểm tra logic chuyển trạng thái (Cancelled = 5)
            var currentStatusId = order.StatusId;
            var newStatusId = dto.StatusId;

            if (currentStatusId == 5 && newStatusId != 5) // Không thể chuyển trạng thái khỏi 'Cancelled'
            {
                return (false, "Không thể thay đổi trạng thái của đơn hàng đã hủy.");
            }

            // II. Cập nhật và lưu

            // 4. Cập nhật Order Entity
            order.StatusId = newStatusId; // Gán ID trực tiếp
            order.UpdatedAt = DateTime.UtcNow;

            // 5. Lưu thay đổi
            if (await _orderRepository.SaveChangesAsync())
            {
                return (true, $"Cập nhật trạng thái đơn hàng (ID {orderId}) thành công.");
            }

            return (false, "Lỗi hệ thống khi lưu thay đổi.");
        }
    }
}
