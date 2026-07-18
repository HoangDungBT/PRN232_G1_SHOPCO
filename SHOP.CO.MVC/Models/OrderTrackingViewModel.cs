using System;
using System.Collections.Generic;

namespace SHOP.CO.MVC.Models
{
    public class OrderTrackingViewModel
    {
        public int OrderId { get; set; }
        public string OrderCode { get; set; } = null!;
        public string OrderStatus { get; set; } = null!;
        public string PaymentStatus { get; set; } = null!;
        public decimal TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public List<OrderTrackingStepViewModel> Steps { get; set; } = new List<OrderTrackingStepViewModel>();
    }
}
