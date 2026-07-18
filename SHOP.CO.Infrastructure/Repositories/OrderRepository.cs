using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SHOP.CO.Application.Repositories;
using SHOP.CO.Domain.Entities;
using SHOP.CO.Infrastructure.Data;

namespace SHOP.CO.Infrastructure.Repositories
{


    public class OrderRepository : BaseRepository<Order>, SHOP.CO.Application.Repositories.IOrderRepository
    {
        public OrderRepository(ShopCoDbContext context) : base(context) { }

        public IQueryable<Order> GetOrdersAsQueryable()
        {
            return _context.Orders.AsQueryable();
        }
        
        public async Task<Order?> GetOrderWithDetailsAsync(int orderId)
        {
            return await _dbSet
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);
        }

        public async Task<List<Order>> GetOrdersByUserIdAsync(int userId)
        {
            return await _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.OrderItems)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task<Order?> GetOrderByIdAsync(int orderId)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .Include(o => o.UserAddress)
                .FirstOrDefaultAsync(o => o.OrderId == orderId) ?? throw new Exception("Order not found");
        }

    public Task AddOrderAsync(Order order) => throw new NotImplementedException();
    public Task<User> GetUserByIdAsync(int id) => throw new NotImplementedException();
    public Task ExecuteInTransactionAsync(Func<Task> action) => throw new NotImplementedException();
    public Task SaveChangesAsync() => throw new NotImplementedException();
    public Task<Order> GetOrderWithItemsAndVariantsByIdAsync(int id) => throw new NotImplementedException();
    public Task<Order> GetOrderByCodeAsync(string code) => throw new NotImplementedException();
    }
}
