using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SHOP.CO.Infrastructure.Data;
using SHOP.CO.Domain.Entities;
using SHOP.CO.Application.DTOs;

namespace SHOP.CO.Application.Services
{
    public interface IAdminNotificationService
    {
        IQueryable<NotificationAdminDto> GetNotificationsQuery();
        Task<NotificationAdminDto> CreateNotificationAsync(SaveNotificationDto request);
        Task<bool> DeleteNotificationAsync(int id);
    }

    public class AdminNotificationService : IAdminNotificationService
    {
        private readonly ShopCoDbContext _context;

        public AdminNotificationService(ShopCoDbContext context)
        {
            _context = context;
        }

        public IQueryable<NotificationAdminDto> GetNotificationsQuery()
        {
            return _context.InteractionLogs
                .Where(x => x.LogType == "Notification" && x.SenderType == "Admin" && x.UserId == null) // Broadcast notifications
                .Select(x => new NotificationAdminDto
                {
                    LogId = x.LogId,
                    Title = x.Title ?? string.Empty,
                    Message = x.Message ?? string.Empty,
                    Status = x.Status ?? "Sent",
                    CreatedAt = x.CreatedAt
                });
        }

        public async Task<NotificationAdminDto> CreateNotificationAsync(SaveNotificationDto request)
        {
            var log = new InteractionLog
            {
                LogType = "Notification",
                Title = request.Title,
                Message = request.Message,
                SenderType = "Admin",
                UserId = null, // null means it's a broadcast to everyone
                Status = "Sent",
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            };

            _context.InteractionLogs.Add(log);
            await _context.SaveChangesAsync();

            return new NotificationAdminDto
            {
                LogId = log.LogId,
                Title = log.Title,
                Message = log.Message,
                Status = log.Status,
                CreatedAt = log.CreatedAt
            };
        }

        public async Task<bool> DeleteNotificationAsync(int id)
        {
            var log = await _context.InteractionLogs
                .FirstOrDefaultAsync(x => x.LogId == id && x.LogType == "Notification" && x.SenderType == "Admin" && x.UserId == null);
            
            if (log == null) return false;

            _context.InteractionLogs.Remove(log);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
