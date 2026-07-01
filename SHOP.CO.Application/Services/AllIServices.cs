using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SHOP.CO.Application.Services
{
    public interface IProductService
    {
        Task<PagedResult<ProductDto>> GetProductsAsync(string? searchTerm, int pageNumber, int pageSize);
    }

    // --- 2. User Service Interface ---
    public interface IUserService
    {
        Task<UserProfileDto> GetProfileAsync(int userId);
        Task<bool> UpdateProfileAsync(int userId, UserProfileDto dto);
        Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto dto);
        Task<bool> DeleteAccountAsync(int userId);
        Task<bool> ToggleNewsletterAsync(int userId, bool subscribe);
    }

    // --- 3. Order Service Interface ---
    public interface IOrderService
    {
        Task<List<OrderHistoryDto>> GetOrderHistoryAsync(int userId);
        Task<OrderHistoryDto> GetOrderDetailAsync(int userId, int orderId);
    }

    // --- 4. Notification Service Interface ---
    public interface INotificationService
    {
        Task<List<NotificationDto>> GetUserNotificationsAsync(int userId);
        Task<bool> MarkAsReadAsync(int userId, int logId);
    }

    // --- 5. Contact Service Interface ---
    public interface IContactService
    {
        Task<bool> SubmitContactFormAsync(ContactDto dto);
    }
}
