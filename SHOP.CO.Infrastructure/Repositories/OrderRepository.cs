using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SHOP.CO.Application.Repositories;
using SHOP.CO.Domain.Entities;
using SHOP.CO.Infrastructure.Data;

namespace SHOP.CO.Infrastructure.Repositories
{


    public class OrderRepository : BaseRepository<Order>, SHOP.CO.Application.Repositories.IOrderRepository, SHOP.CO.Infrastructure.Repositories.IOrderRepository
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

        public async Task<List<Order>> GetOrdersByUserIdAsync(int userId, string? status = null, string? search = null)
        {
            var query = _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.OrderItems)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(o => o.OrderStatus == status);

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(o =>
                    o.OrderCode.Contains(search) ||
                    o.OrderItems.Any(i => i.ProductNameSnapshot.Contains(search)) ||
                    o.ReceiverName.Contains(search));

            return await query
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

        public async Task AddOrderAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _context.Users
                .Include(u => u.UserAddresses)
                .FirstOrDefaultAsync(u => u.UserId == id);
        }

        public async Task ExecuteInTransactionAsync(Func<Task> action)
        {
            var executionStrategy = _context.Database.CreateExecutionStrategy();
            await executionStrategy.ExecuteAsync(async () =>
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    await action();
                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            });
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<Order?> GetOrderWithItemsAndVariantsByIdAsync(int id)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.ProductVariant)
                .FirstOrDefaultAsync(o => o.OrderId == id);
        }

        public async Task<Order?> GetOrderByCodeAsync(string code)
        {
            return await _context.Orders.FirstOrDefaultAsync(o => o.OrderCode == code);
        }
    }
}
