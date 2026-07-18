using System;

namespace SHOP.CO.MVC.Models
{
    public class OrderTrackingStepViewModel
    {
        public string Status { get; set; } = null!;
        public string Description { get; set; } = null!;
        public bool Completed { get; set; }
        public DateTime? Time { get; set; }
    }
}
