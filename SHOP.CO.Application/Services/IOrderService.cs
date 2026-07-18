using System.Collections.Generic;
using System.Threading.Tasks;
using SHOP.CO.Application.DTOs;

namespace SHOP.CO.Application.Services
{
    public interface IOrderService
    {
        Task<CheckoutResponseDto> CheckoutAsync(CheckoutRequestDto requestDto);
        Task<List<OrderDto>> GetOrdersByUserIdAsync(int userId);
        Task<OrderDto?> GetOrderByIdAsync(int orderId);
        Task<bool> CancelOrderAsync(int orderId, int userId, string cancelReason);
        Task<OrderTrackingDto?> GetOrderTrackingAsync(int orderId);
        Task<bool> SubmitReturnRequestAsync(int orderId, int userId, string reason, string description);
        Task<System.Collections.Generic.List<UserAddressDto>> GetUserAddressesAsync(int userId);
    }
}
