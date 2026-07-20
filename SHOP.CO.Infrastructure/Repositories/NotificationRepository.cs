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
        void AddNotificationReadReceipt(int userId, int logId);
        Task SaveChangesAsync();
    }

    public class NotificationRepository : INotificationRepository
    {
        private readonly ShopCoDbContext _context;
        public NotificationRepository(ShopCoDbContext context) => _context = context;

        public async Task<List<InteractionLog>> GetUserNotificationsAsync(int userId)
        {
            var readGlobalIds = await _context.InteractionLogs
                .Where(l => l.UserId == userId && l.LogType == "Audit" && l.ActionName == "NotificationRead")
                .Select(l => l.Message)
                .ToListAsync();

            var notifications = await _context.InteractionLogs
                .Where(l => (l.UserId == userId || l.UserId == null) && l.LogType == "Notification")
                .OrderByDescending(l => l.CreatedAt)
                .ToListAsync();

            foreach (var n in notifications)
            {
                if (n.UserId == null && readGlobalIds.Contains(n.LogId.ToString()))
                {
                    n.IsRead = true;
                }
            }
            return notifications;
        }

        public void AddNotificationReadReceipt(int userId, int logId)
        {
            var log = new InteractionLog
            {
                UserId = userId,
                LogType = "Audit",
                ActionName = "NotificationRead",
                Title = "ReadReceipt",
                Message = logId.ToString(),
                CreatedAt = DateTime.UtcNow,
                IsRead = true
            };
            _context.InteractionLogs.Add(log);
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
