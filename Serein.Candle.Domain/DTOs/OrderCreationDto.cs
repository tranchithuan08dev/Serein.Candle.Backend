using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serein.Candle.Domain.DTOs
{
   
        public class OrderCreationDto
        {
            // Thông tin địa chỉ giao hàng (sẽ được tạo mới hoặc dùng AddressId có sẵn)
            [Required]
            public string FullName { get; set; } = string.Empty;

            [Required]
            [Phone]
            public string Phone { get; set; } = string.Empty;

            [Required]
            public string AddressLine { get; set; } = string.Empty;

            public string? City { get; set; }
            public string? District { get; set; }
            public string? Ward { get; set; }

            // Chi tiết đơn hàng
            [Required]
            [MinLength(1, ErrorMessage = "Đơn hàng phải có ít nhất một sản phẩm.")]
            public ICollection<OrderItemDto> Items { get; set; } = new List<OrderItemDto>();

            // Phương thức thanh toán
            [Required]
            public int PaymentMethodId { get; set; }

            public string? Note { get; set; }
            public string? VoucherCode { get; set; }
        }

        public class OrderItemDto
        {
            [Required]
            public int ProductId { get; set; }

            [Range(1, int.MaxValue)]
            public int Quantity { get; set; }
        }
}

