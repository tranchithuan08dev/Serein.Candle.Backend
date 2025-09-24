using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serein.Candle.Domain.DTOs
{
   
        public class CartDetailDto
        {
            public int CartId { get; set; }
            public int UserId { get; set; }
            public decimal TotalAmount { get; set; }
            public List<CartItemDto> Items { get; set; } = new List<CartItemDto>();
        }

        public class CartItemDto
        {
            public int ProductId { get; set; }
            public string ProductName { get; set; } = string.Empty;
            public string ProductSku { get; set; } = string.Empty;
            public int Quantity { get; set; }
            public decimal PriceAtAdd { get; set; }
            public string ImageUrl { get; set; } = string.Empty;
        }
    }

