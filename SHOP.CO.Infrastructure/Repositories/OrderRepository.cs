using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SHOP.CO.Infrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ShopCoDbContext _context;
        public OrderRepository(ShopCoDbContext context) => _context = context;

        public async Task<List<Order>> GetOrdersByUserIdAsync(int userId)
        {
            return await _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.OrderItems)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task<Order> GetOrderByIdAsync(int orderId)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .Include(o => o.UserAddress)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);
        }
    }
}
