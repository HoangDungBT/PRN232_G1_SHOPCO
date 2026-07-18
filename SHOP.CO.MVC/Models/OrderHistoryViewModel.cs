using System;
using System.Collections.Generic;

namespace SHOP.CO.MVC.Models
{
    public class OrderHistoryViewModel
    {
        public int Id { get; set; }
        public string OrderStatus { get; set; } = string.Empty;
        public decimal FinalAmount { get; set; }
        public DateTime CreatedAt { get; set; }
        public int TotalItems { get; set; }
        public string OrderCode { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public int OrderId { get; set; }
        public List<OrderItemViewModel> Items { get; set; } = new List<OrderItemViewModel>();
    }

    public class OrderItemViewModel
    {
        public string ImageUrlSnapshot { get; set; } = string.Empty;
        public string ProductNameSnapshot { get; set; } = string.Empty;
        public string SizeSnapshot { get; set; } = string.Empty;
        public string ColorSnapshot { get; set; } = string.Empty;
        public decimal LineTotal { get; set; }
        public string DisplayImageUrl { get; set; } = string.Empty;
        public string SkuSnapshot { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
    }
}
