using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SHOP.CO.Infrastructure.Repositories
{
    public interface IProductRepository
    {
        Task<(List<Product> Items, int TotalCount)> GetPagedProductAsync(string? searchTerm, int pageNumber, int pageSize);

    }

    // --- 2. User Repository Interface ---
    public interface IUserRepository
    {
        Task<User> GetUserByIdAsync(int userId);
        Task<User> GetUserByEmailAsync(string email);
        void UpdateUser(User user);
        void DeleteUser(User user);
        Task SaveChangesAsync();
    }

    // --- 3. Order Repository Interface ---
    public interface IOrderRepository
    {
        Task<List<Order>> GetOrdersByUserIdAsync(int userId);
        Task<Order> GetOrderByIdAsync(int orderId);
    }

    // --- 4. Notification Repository Interface ---
    public interface INotificationRepository
    {
        Task<List<InteractionLog>> GetUserNotificationsAsync(int userId);
        Task<InteractionLog> GetNotificationByIdAsync(int logId);
        void MarkAsRead(InteractionLog log);
        Task SaveChangesAsync();
    }

    // --- 5. Contact Repository Interface ---
    public interface IContactRepository
    {
        void AddContactLog(InteractionLog log);
        Task SaveChangesAsync();
    }
}
