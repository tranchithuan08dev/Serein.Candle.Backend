using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serein.Candle.Domain.DTOs
{
    public class OrderAdminSummaryDto
    {
        public int OrderId { get; set; }
        public string OrderCode { get; set; } = string.Empty;
        public string StatusName { get; set; } = string.Empty;
        public string PaymentMethodName { get; set; } = string.Empty;

        // Thông tin khách hàng (cần thiết cho Admin)
        public int? UserId { get; set; } // Có thể null nếu là Guest Order
       
        public string CustomerFullName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;

        public string RecipientAddress { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal ShippingFee { get; set; }
        public DateTime CreatedAt { get; set; }

         //Có thể include Items nếu muốn hiển thị chi tiết sản phẩm ngay trên list (Tùy chọn)
         public ICollection<OrderItemDetailDto> Items { get; set; } = new List<OrderItemDetailDto>();
    }
}
