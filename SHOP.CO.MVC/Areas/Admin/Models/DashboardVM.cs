namespace SHOP.CO.MVC.Areas.Admin.Models
{
    public class DashboardVM
    {
        public int TotalUsers { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public int LowStockProducts { get; set; }
    }
}
