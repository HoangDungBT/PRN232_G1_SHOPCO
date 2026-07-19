using System.Collections.Generic;
using System.Threading.Tasks;
using SHOP.CO.Application.DTOs;
using SHOP.CO.Infrastructure.Data;
using SHOP.CO.Domain.Entities;
using SHOP.CO.Application.Common;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;

namespace SHOP.CO.Application.Services
{
    public interface IOrderService
    {
        Task<CheckoutResponseDto> CheckoutAsync(CheckoutRequestDto requestDto);
        Task<List<OrderDto>> GetOrdersByUserIdAsync(int userId, string? status = null, string? search = null);
        Task<OrderDto?> GetOrderByIdAsync(int orderId);
        Task<bool> CancelOrderAsync(int orderId, int userId, string cancelReason);
        Task<OrderTrackingDto?> GetOrderTrackingAsync(int orderId);
        Task<bool> SubmitReturnRequestAsync(int orderId, int userId, string reason, string description);
        Task<System.Collections.Generic.List<UserAddressDto>> GetUserAddressesAsync(int userId);
        
        Task<IEnumerable<Order>> GetOrderHistoryAsync(int userId);
        Task<Order> GetOrderDetailAsync(int orderId, int userId);
    }
}
