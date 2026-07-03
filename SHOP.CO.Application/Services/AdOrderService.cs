using Microsoft.EntityFrameworkCore;
using SHOP.CO.Application.Common;
using SHOP.CO.Domain.Entities;
using SHOP.CO.Infrastructure.Persistence;
using System.Linq;
using System.Threading.Tasks;

namespace SHOP.CO.Application.Services
{
    public interface IOrderAdminService
    {
        IQueryable<OrderDto> GetOrdersODataQuery();
        Task<ResultModel<OrderDetailAdminDto>> GetOrderDetailsAsync(int orderId);
        Task<ResultModel<bool>> UpdateOrderStatusAsync(int orderId, UpdateOrderStatusDto dto);
        Task<ResultModel<bool>> MarkAsPaidAsync(int orderId);
    }

    public class OrderAdminService : IOrderAdminService
    {
        private readonly IOrderRepository _orderRepo;
        private readonly ShopCoDbContext _context; // Inject DbContext để chọc vào Tồn kho

        public OrderAdminService(IOrderRepository orderRepo, ShopCoDbContext context)
        {
            _orderRepo = orderRepo;
            _context = context;
        }

        public IQueryable<OrderDto> GetOrdersODataQuery()
        {
            return _orderRepo.GetOrdersAsQueryable().Select(o => new OrderDto
            {
                OrderId = o.OrderId,
                OrderCode = o.OrderCode,
                ReceiverName = o.ReceiverName,
                TotalAmount = o.TotalAmount,
                OrderStatus = o.OrderStatus,
                PaymentStatus = o.PaymentStatus,
                CreatedAt = o.CreatedAt
            });
        }

        public async Task<ResultModel<OrderDetailAdminDto>> GetOrderDetailsAsync(int orderId)
        {
            try
            {
                var order = await _orderRepo.GetOrderWithDetailsAsync(orderId);
                if (order == null) return ResultModel<OrderDetailAdminDto>.Error("Không tìm thấy đơn hàng", 404);

                var data = new OrderDetailAdminDto
                {
                    OrderId = order.OrderId,
                    OrderCode = order.OrderCode,
                    ReceiverName = order.ReceiverName,
                    ReceiverPhone = order.ReceiverPhone,
                    ShippingAddressText = order.ShippingAddressText,
                    TotalAmount = order.TotalAmount,
                    SubtotalAmount = order.SubtotalAmount,
                    ShippingFee = order.ShippingFee,
                    DiscountAmount = order.DiscountAmount,
                    OrderStatus = order.OrderStatus,
                    PaymentStatus = order.PaymentStatus,
                    CustomerNote = order.CustomerNote,
                    StaffNote = order.StaffNote,
                    CancelReason = order.CancelReason,
                    CreatedAt = order.CreatedAt,
                    Items = order.OrderItems.Select(i => new OrderItemAdminDto
                    {
                        VariantId = i.VariantId,
                        ProductName = i.ProductNameSnapshot,
                        Sku = i.SkuSnapshot,
                        Size = i.SizeSnapshot,
                        Color = i.ColorSnapshot,
                        ImageUrl = i.ImageUrlSnapshot,
                        Quantity = i.Quantity,
                        UnitPrice = i.UnitPrice,
                        SalePrice = i.SalePrice ?? i.UnitPrice,
                        LineTotal = i.LineTotal
                    }).ToList()
                };

                return ResultModel<OrderDetailAdminDto>.Success(data);
            }
            catch (Exception ex) { return ResultModel<OrderDetailAdminDto>.Exception(ex); }
        }

        public async Task<ResultModel<bool>> UpdateOrderStatusAsync(int orderId, UpdateOrderStatusDto dto)
        {
            try
            {
                var order = await _orderRepo.GetOrderWithDetailsAsync(orderId);
                if (order == null) return ResultModel<bool>.Error("Không tìm thấy đơn hàng", 404);

                if (order.OrderStatus == "Completed" || order.OrderStatus == "Canceled")
                    return ResultModel<bool>.Error("Đơn hàng đã đóng, không thể thay đổi trạng thái!", 400);

                // 🟢 LOGIC: TRỪ TỒN KHO KHI CHUYỂN SANG ĐANG GIAO (Shipping)
                if (dto.Status == "Shipping" && order.OrderStatus != "Shipping")
                {
                    foreach (var item in order.OrderItems)
                    {
                        var variant = await _context.ProductVariants.FindAsync(item.VariantId);
                        if (variant != null)
                        {
                            if (variant.StockQuantity < item.Quantity)
                                return ResultModel<bool>.Error($"Sản phẩm '{item.SkuSnapshot}' không đủ tồn kho để giao hàng!", 400);

                            variant.StockQuantity -= item.Quantity; // Trừ tồn kho
                        }
                    }
                }

                // 🟢 LOGIC: HOÀN TRẢ TỒN KHO NẾU KHÁCH HỦY KHI ĐANG GIAO (Boom hàng)
                if (dto.Status == "Canceled" && order.OrderStatus == "Shipping")
                {
                    foreach (var item in order.OrderItems)
                    {
                        var variant = await _context.ProductVariants.FindAsync(item.VariantId);
                        if (variant != null) variant.StockQuantity += item.Quantity;
                    }
                }

                order.OrderStatus = dto.Status;
                if (dto.Status == "Canceled")
                {
                    order.CanceledAt = DateTime.UtcNow;
                    order.CancelReason = dto.CancelReason ?? "Admin hủy đơn";
                }
                if (dto.Status == "Completed")
                {
                    order.CompletedAt = DateTime.UtcNow;
                    order.PaymentStatus = "Paid"; // Giao thành công coi như thu tiền xong (COD)
                }

                order.UpdatedAt = DateTime.UtcNow;
                await _orderRepo.UpdateOrderAsync(order);

                return ResultModel<bool>.Success(true, $"Cập nhật trạng thái thành: {dto.Status}");
            }
            catch (Exception ex) { return ResultModel<bool>.Exception(ex); }
        }

        public async Task<ResultModel<bool>> MarkAsPaidAsync(int orderId)
        {
            try
            {
                var order = await _orderRepo.GetOrderWithDetailsAsync(orderId);
                if (order == null) return ResultModel<bool>.Error("Không tìm thấy đơn hàng", 404);

                order.PaymentStatus = "Paid";
                order.UpdatedAt = DateTime.UtcNow;
                await _orderRepo.UpdateOrderAsync(order);

                return ResultModel<bool>.Success(true, "Đã cập nhật trạng thái thanh toán thành Đã thanh toán!");
            }
            catch (Exception ex) { return ResultModel<bool>.Exception(ex); }
        }
    }
   
}
