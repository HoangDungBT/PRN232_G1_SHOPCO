using Microsoft.EntityFrameworkCore;
using SHOP.CO.Application.Common;
using SHOP.CO.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SHOP.CO.Application.Services
{
    public interface IAdminService
    {
        Task<ResultModel<DashboardSummaryDto>> GetDashBoardSummaryAsync();
        Task<ResultModel<List<RevenueByDayDto>>> GetRevenueChartAsync(int days);
        Task<ResultModel<bool>> AdjustStockAsync(int variantId, int quantityChange, string reason, string logType);
        IQueryable<ProductVariant> GetInventoryODataQuery();
    }

    public class AdminService : IAdminService
    {
        private readonly ShopCoDbContext _context;
        public AdminService(ShopCoDbContext context)
        {
            _context = context;
        }

        public async Task<ResultModel<DashboardSummaryDto>> GetDashBoardSummaryAsync()
        {
            try
            {
                var totalUsers = await _context.Users.CountAsync(u => u.Role == "Customer");
                var totalOrders = await _context.Orders.CountAsync();

                var totalRevenue = await _context.Orders
                    .Where(o => o.OrderStatus == "Completed")
                    .SumAsync(o => o.TotalAmount);

                var lowStock = await _context.ProductVariants
                    .CountAsync(v => v.StockQuantity <= v.LowStockThreshold);
                var data = new DashboardSummaryDto
                {
                    TotalUsers = totalUsers,
                    TotalOrders = totalOrders,
                    LowStockProducts = lowStock,
                    TotalRevenue = totalRevenue
                };
                return ResultModel<DashboardSummaryDto>.Success(data, "Lấy dữ liệu thông kê thành công");

            }
            catch (Exception ex)
            {

                return ResultModel<DashboardSummaryDto>.Exception(ex);
            }

        }

        public async Task<ResultModel<List<RevenueByDayDto>>> GetRevenueChartAsync(int days)
        {
            try
            {
                var endDate = DateTime.UtcNow.Date;
                var startDate = endDate.AddDays(-days + 1);
                var order = await _context.Orders
                    .Where(o => o.OrderStatus == "Completed" && o.CreatedAt >= startDate && o.CreatedAt <= endDate.AddDays(1))
                    .ToListAsync();

                var groupedRevenue = order
                    .GroupBy(o => o.CreatedAt.Date)
                    .ToDictionary(g => g.Key, g => g.Sum(o => o.TotalAmount));

                var result = new List<RevenueByDayDto>();
                for (int i = 0; i < days; i++)
                {
                    var currentDate = startDate.AddDays(i);
                    result.Add(new RevenueByDayDto
                    {
                        Date = currentDate.ToString("dd/MM"),
                        Revenue = groupedRevenue.ContainsKey(currentDate)
                        ? groupedRevenue[currentDate] : 0
                    });
                }
                return ResultModel<List<RevenueByDayDto>>.Success(result);

            }
            catch (Exception ex)
            {
                return ResultModel<List<RevenueByDayDto>>.Exception(ex);
            }
        }
        public IQueryable<ProductVariant> GetInventoryODataQuery()
        {
            return _context.ProductVariants
                .Include(v => v.Product) // Để lấy tên sản phẩm hiển thị ra View
                .AsQueryable();
        }
        public async Task<ResultModel<bool>> AdjustStockAsync(int variantId, int quantityChange, string reason, string logType = "StockMovement")
        {
            try
            {
                // 1. Tìm biến thể
                var variant = await _context.ProductVariants.FindAsync(variantId);
                if (variant == null) return ResultModel<bool>.Error("Không tìm thấy biến thể", 404);

                // 2. Cập nhật tồn kho hiện tại
                variant.StockQuantity += quantityChange;

                // 3. Ghi vào InteractionLog
                var log = new InteractionLog
                {
                    VariantId = variantId,
                    ProductId = variant.ProductId,
                    LogType = logType,
                    QuantityChanged = quantityChange,
                    Message = reason,
                    CreatedAt = DateTime.UtcNow,
                    Status = "Success"
                };

                _context.InteractionLogs.Add(log);
                await _context.SaveChangesAsync();

                return ResultModel<bool>.Success(true, "Cập nhật kho thành công");
            }
            catch (Exception ex)
            {
                var innerEx = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return ResultModel<bool>.Error(innerEx,500);
            }
        }
    }
}
