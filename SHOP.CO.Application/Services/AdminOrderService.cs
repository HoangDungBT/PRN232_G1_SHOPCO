namespace SHOP.CO.Application.Services
{
    public interface IAdminOrderService
    {
        IQueryable<OrderDto> GetOrdersODataQuery();
        Task<ResultModel<OrderDetailAdminDto>> GetOrderDetailsAsync(int orderId);
        Task<ResultModel<bool>> UpdateOrderStatusAsync(int orderId, UpdateOrderStatusDto dto);
        Task<ResultModel<bool>> MarkAsPaidAsync(int orderId);
    }

    public class AdminOrderService : IAdminOrderService
    {
        private readonly IOrderRepository _orderRepo;
        private readonly ShopCoDbContext _context;
        private readonly IMapper _mapper; // 🟢 Inject AutoMapper

        public AdminOrderService(IOrderRepository orderRepo, ShopCoDbContext context, IMapper mapper)
        {
            _orderRepo = orderRepo;
            _context = context;
            _mapper = mapper;
        }

        public IQueryable<OrderDto> GetOrdersODataQuery()
        {
            // 🟢 TỰ ĐỘNG MAP        
            return _orderRepo.GetQueryable()
                             .ProjectTo<OrderDto>(_mapper.ConfigurationProvider);
        }

        public async Task<ResultModel<OrderDetailAdminDto>> GetOrderDetailsAsync(int orderId)
        {
            try
            {
                var order = await _orderRepo.GetOrderWithDetailsAsync(orderId);
                if (order == null) return ResultModel<OrderDetailAdminDto>.Error("Không tìm thấy đơn hàng", 404);

                // 🟢 TỰ ĐỘNG MAP TỪ ĐƠN HÀNG XUỐNG CẢ CHI TIẾT SẢN PHẨM TRONG 1 DÒNG
                var data = _mapper.Map<OrderDetailAdminDto>(order);

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

                order.OrderStatus = dto.Status;
                if (dto.Status == "Canceled")
                {
                    order.CanceledAt = DateTime.UtcNow;
                    order.CancelReason = dto.CancelReason ?? "Admin hủy đơn";
                }
                if (dto.Status == "Completed")
                {
                    order.CompletedAt = DateTime.UtcNow;
                    order.PaymentStatus = "Paid";
                }

                order.UpdatedAt = DateTime.UtcNow;
                await _orderRepo.UpdateAsync(order); // BaseRepo

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
                await _orderRepo.UpdateAsync(order); // BaseRepo

                return ResultModel<bool>.Success(true, "Đã cập nhật trạng thái thanh toán!");
            }
            catch (Exception ex) { return ResultModel<bool>.Exception(ex); }
        }
    }
}