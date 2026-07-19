using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SHOP.CO.Application.Services
{
    public interface IOrderService
    {
        Task<List<OrderHistoryDto>> GetOrderHistoryAsync(int userId);
        Task<OrderHistoryDto> GetOrderDetailAsync(int userId, int orderId);
    }

    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;

        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<List<OrderHistoryDto>> GetOrderHistoryAsync(int userId)
        {
            var orders = await _orderRepository.GetOrdersByUserIdAsync(userId);

            return orders.Select(o => new OrderHistoryDto
            {
                OrderId = o.OrderId,
                OrderCode = o.OrderCode,
                TotalAmount = o.TotalAmount,
                OrderStatus = o.OrderStatus,
                CreatedAt = o.CreatedAt,
                Items = o.OrderItems.Select(oi => new OrderItemDto
                {
                    ProductNameSnapshot = oi.ProductNameSnapshot,
                    SizeSnapshot = oi.SizeSnapshot,
                    ColorSnapshot = oi.ColorSnapshot,
                    ImageUrlSnapshot = oi.ImageUrlSnapshot,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice,
                    LineTotal = oi.LineTotal
                }).ToList()
            }).ToList();
        }

        public async Task<OrderHistoryDto> GetOrderDetailAsync(int userId, int orderId)
        {
            var order = await _orderRepository.GetOrderByIdAsync(orderId);
            if (order == null || order.UserId != userId) return null!; // Kiểm tra quyền sở hữu đơn hàng

            return new OrderHistoryDto
            {
                OrderId = order.OrderId,
                OrderCode = order.OrderCode,
                TotalAmount = order.TotalAmount,
                OrderStatus = order.OrderStatus,
                CreatedAt = order.CreatedAt,
                Items = order.OrderItems.Select(oi => new OrderItemDto
                {
                    ProductNameSnapshot = oi.ProductNameSnapshot,
                    SizeSnapshot = oi.SizeSnapshot,
                    ColorSnapshot = oi.ColorSnapshot,
                    ImageUrlSnapshot = oi.ImageUrlSnapshot,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice,
                    LineTotal = oi.LineTotal
                }).ToList()
            };
        }
    }
}
