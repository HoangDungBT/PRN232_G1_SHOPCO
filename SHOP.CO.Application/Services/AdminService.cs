using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SHOP.CO.Application.Common;
using SHOP.CO.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SHOP.CO.Application.Services
{
    public interface IAdminDashboardService
    {
        Task<ResultModel<DashboardSummaryDto>> GetDashBoardSummaryAsync();
        Task<ResultModel<List<RevenueByDayDto>>> GetRevenueChartAsync(int days);
        Task<ResultModel<bool>> AdjustStockAsync(int variantId, int quantityChange, string reason, string logType);
        IQueryable<ProductVariant> GetInventoryODataQuery();



    }

    public class AdminDashboardService : IAdminDashboardService
    {
        private readonly ShopCoDbContext _context;
        private readonly IEmailSender _emailSender;
        private readonly EmailSettings _emailSettings;

        // 🟢 1. CẬP NHẬT CONSTRUCTOR ĐỂ GỌI ĐƯỢC EMAIL SENDER
        public AdminDashboardService(
            ShopCoDbContext context,
            IEmailSender emailSender,
            IOptions<EmailSettings> emailSettings)
        {
            _context = context;
            _emailSender = emailSender;
            _emailSettings = emailSettings.Value;
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

        public async Task<ResultModel<bool>> AdjustStockAsync(int variantId, int quantityChange, string reason, string logType)
        {
            try
            {
                string[] allowedLogTypes = { "Chatbot", "Notification", "Email", "Audit", "StockMovement", "StockAlert", "ReportExport" };
                string finalLogType = allowedLogTypes.Contains(logType) ? logType : "StockMovement";

                // Thay FindAsync bằng FirstOrDefaultAsync + Include để lấy tên Product gửi Email
                var variant = await _context.ProductVariants
                    .Include(v => v.Product)
                    .FirstOrDefaultAsync(v => v.VariantId == variantId);

                if (variant == null) return ResultModel<bool>.Error("Không tìm thấy biến thể", 404);

                // Lưu lại số lượng cũ để check xem có phải VỪA MỚI rớt xuống ngưỡng báo động không
                int oldStock = variant.StockQuantity;
                int threshold = variant.LowStockThreshold;

                // Cập nhật tồn kho hiện tại
                variant.StockQuantity += quantityChange;
                int newStock = variant.StockQuantity;

                // Ghi Log Chuyển động kho (StockMovement)
                var log = new InteractionLog
                {
                    VariantId = variantId,
                    ProductId = variant.ProductId,
                    LogType = finalLogType,
                    QuantityChanged = quantityChange,
                    Message = $"[OriginalType: {logType}] {reason}",
                    CreatedAt = DateTime.UtcNow,
                    Status = "Success"
                };
                _context.InteractionLogs.Add(log);

                // 🟢 3. LOGIC GỬI EMAIL CẢNH BÁO TỒN KHO 🟢
                // Chỉ gửi khi: Số lượng giảm (<0) VÀ Vượt qua ngưỡng báo động
                if (quantityChange < 0 && oldStock > threshold && newStock <= threshold)
                {
                    // Tạo một Log riêng cho việc cảnh báo
                    var alertLog = new InteractionLog
                    {
                        VariantId = variantId,
                        ProductId = variant.ProductId,
                        LogType = "StockAlert",
                        SenderType = "System",
                        Title = "Cảnh báo tồn kho thấp",
                        Message = $"Sản phẩm {variant.Product?.ProductName} (SKU: {variant.Sku}) chỉ còn {newStock} sản phẩm.",
                        IsRead = false,
                        CreatedAt = DateTime.UtcNow,
                        Status = "Sent"
                    };
                    _context.InteractionLogs.Add(alertLog);

                    // Bắn Email (Dùng _ = Task.Run để gửi ngầm, không làm chậm quá trình của Admin)
                    string emailBody = $@"
                        <div style='font-family: Arial, sans-serif; border: 1px solid #ddd; padding: 20px; border-radius: 5px;'>
                            <h2 style='color: #dc3545;'><i class='fas fa-exclamation-triangle'></i> Cảnh báo Hết hàng</h2>
                            <p>Hệ thống SHOP.CO thông báo một sản phẩm vừa chạm ngưỡng tồn kho thấp:</p>
                            <ul>
                                <li><strong>Sản phẩm:</strong> {variant.Product?.ProductName}</li>
                                <li><strong>Phân loại (Màu/Size):</strong> {variant.Color} - {variant.Size}</li>
                                <li><strong>Mã SKU:</strong> {variant.Sku}</li>
                                <li><strong style='color: #dc3545;'>Tồn kho hiện tại: {newStock}</strong> (Ngưỡng: {threshold})</li>
                            </ul>
                            <p>Vui lòng kiểm tra và nhập thêm hàng sớm nhất có thể!</p>
                        </div>";

                    if (!string.IsNullOrEmpty(_emailSettings.AdminEmail))
                    {
                        _ = _emailSender.SendEmailAsync(_emailSettings.AdminEmail, $"[CẢNH BÁO KHO] Sắp hết hàng: {variant.Sku}", emailBody);
                    }
                }

                await _context.SaveChangesAsync();

                return ResultModel<bool>.Success(true, "Cập nhật kho thành công");
            }
            catch (Exception ex)
            {
                var innerEx = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return ResultModel<bool>.Error(innerEx, 500);
            }
        }

    }
}
