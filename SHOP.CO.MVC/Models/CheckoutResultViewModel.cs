using System;

namespace SHOP.CO.MVC.Models
{
    public class CheckoutResultViewModel
    {
        public int OrderId { get; set; }
        public string OrderCode { get; set; } = null!;
        public string OrderStatus { get; set; } = null!;
        public decimal TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
