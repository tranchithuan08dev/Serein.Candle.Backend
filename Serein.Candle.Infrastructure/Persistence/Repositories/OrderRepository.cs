using Microsoft.EntityFrameworkCore;
using Serein.Candle.Domain.Entities;
using Serein.Candle.Domain.Interfaces;
using Serein.Candle.Infrastructure.Persistence.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serein.Candle.Infrastructure.Persistence.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly CandleShopDbContext _context;

        public OrderRepository(CandleShopDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Order>> GetOrdersByUserIdAsync(int userId)
        {
            return await _context.Orders
                                 .Where(o => o.UserId == userId)
                                 .Include(o => o.Status)
                                 .OrderByDescending(o => o.CreatedAt)
                                 .ToListAsync();
        }

        public async Task<Order?> GetOrderDetailsByIdAsync(int orderId)
        {
            return await _context.Orders
                                 .Where(o => o.OrderId == orderId)
                                 .Include(o => o.OrderItems).ThenInclude(oi => oi.Product)
                                 .Include(o => o.Status)
                                 .Include(o => o.PaymentMethod)
                                 .Include(o => o.ShippingAddress)
                                 .FirstOrDefaultAsync();
        }

        public async Task AddAddressAsync(CustomerAddress address)
        {
            await _context.CustomerAddresses.AddAsync(address);
        }

        public async Task AddOrderAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
        }

        public async Task<Product?> GetProductByIdAsync(int productId)
        {
            return await _context.Products.FindAsync(productId);
        }

        public async Task<Voucher?> GetVoucherByCodeAsync(string code)
        {
            return await _context.Vouchers.FirstOrDefaultAsync(v => v.Code == code && v.IsActive);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
