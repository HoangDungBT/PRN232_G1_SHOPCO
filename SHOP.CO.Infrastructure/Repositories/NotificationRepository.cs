using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SHOP.CO.Infrastructure.Repositories
{
    public interface INotificationRepository
    {
        Task<List<InteractionLog>> GetUserNotificationsAsync(int userId);
        Task<InteractionLog> GetNotificationByIdAsync(int logId);
        void MarkAsRead(InteractionLog log);
        Task SaveChangesAsync();
    }

    public class NotificationRepository : INotificationRepository
    {
        private readonly ShopCoDbContext _context;
        public NotificationRepository(ShopCoDbContext context) => _context = context;

        public async Task<List<InteractionLog>> GetUserNotificationsAsync(int userId)
        {
            return await _context.InteractionLogs
                .Where(l => l.UserId == userId && l.LogType == "Notification")
                .OrderByDescending(l => l.CreatedAt)
                .ToListAsync();
        }

        public async Task<InteractionLog> GetNotificationByIdAsync(int logId)
        {
            return await _context.InteractionLogs.FindAsync(logId);
        }

        public void MarkAsRead(InteractionLog log)
        {
            log.IsRead = true;
            log.ReadAt = DateTime.UtcNow;
            _context.InteractionLogs.Update(log);
        }

        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
    }
}
