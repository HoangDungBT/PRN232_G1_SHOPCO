using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;
using SHOP.CO.Domain.Entities;

namespace SHOP.CO.Infrastructure.Repositories
{
    public interface IOrderRepository
    {
        Task AddOrderAsync(Order order);
        Task<User?> GetUserByIdAsync(int userId);
        Task ExecuteInTransactionAsync(Func<Task> action);
        Task SaveChangesAsync();
        Task<List<Order>> GetOrdersByUserIdAsync(int userId);
        Task<Order?> GetOrderByIdAsync(int orderId);
        Task<Order?> GetOrderWithItemsAndVariantsByIdAsync(int orderId);
        Task<Order?> GetOrderByCodeAsync(string orderCode);
    }
}
