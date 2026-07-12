using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SHOP.CO.Application.Services
{
    public interface INotificationService
    {
        Task<List<NotificationDto>> GetUserNotificationsAsync(int userId);
        Task<bool> MarkAsReadAsync(int userId, int logId);
    }

    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;

        public NotificationService(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public async Task<List<NotificationDto>> GetUserNotificationsAsync(int userId)
        {
            var logs = await _notificationRepository.GetUserNotificationsAsync(userId);
            return logs.Select(l => new NotificationDto
            {
                LogId = l.LogId,
                Title = l.Title ?? string.Empty,
                Message = l.Message ?? string.Empty,
                IsRead = l.IsRead,
                CreatedAt = l.CreatedAt
            }).ToList();
        }

        public async Task<bool> MarkAsReadAsync(int userId, int logId)
        {
            var log = await _notificationRepository.GetNotificationByIdAsync(logId);
            if (log == null || log.UserId != userId) return false; // Kiểm tra quyền sở hữu

            _notificationRepository.MarkAsRead(log);
            await _notificationRepository.SaveChangesAsync();
            return true;
        }
    }
}
