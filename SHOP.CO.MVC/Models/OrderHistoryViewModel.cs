namespace SHOP.CO.MVC.Models
{
    public class OrderItemViewModel
    {
        public string ProductNameSnapshot { get; set; }
        public string SizeSnapshot { get; set; }
        public string ColorSnapshot { get; set; }
        public string ImageUrlSnapshot { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal { get; set; }
    }

    public class OrderHistoryViewModel
    {
        public int OrderId { get; set; }
        public string OrderCode { get; set; }
        public decimal TotalAmount { get; set; }
        public string OrderStatus { get; set; }
        public string CreatedAt { get; set; }
        public List<OrderItemViewModel> Items { get; set; }
    }
}