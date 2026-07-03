using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SHOP.CO.Application.DTOs
{
    public class DashboardSummaryDto
    {
        public int TotalUsers { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public int LowStockProducts { get; set; }
    }

    public class RevenueByDayDto
    {
        public string Date { get; set; } = string.Empty;
        public decimal Revenue { get; set; }
    }
    public static class LogTypes
    {
        public const string StockImport = "StockImport"; // Nhập kho
        public const string StockExport = "StockExport"; // Xuất kho (hư hỏng, hủy)
        public const string StockAdjustment = "StockAdjustment"; // Điều chỉnh kho (cân lại kho)
        public const string SaleDeduction = "SaleDeduction"; // Trừ kho khi có đơn hàng
    }

    public class StockAdjustmentDto
    {
        public int VariantId { get; set; }
        public int QuantityChange { get; set; } // Dương là nhập, Âm là xuất
        public string Reason { get; set; } = string.Empty;
    }
}
