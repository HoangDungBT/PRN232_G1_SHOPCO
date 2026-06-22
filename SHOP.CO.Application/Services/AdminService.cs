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
    }
}
