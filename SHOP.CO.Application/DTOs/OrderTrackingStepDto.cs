using System;

namespace SHOP.CO.Application.DTOs
{
    public class OrderTrackingStepDto
    {
        public string Status { get; set; } = null!;

        public string Description { get; set; } = null!;

        public bool Completed { get; set; }

        public DateTime? Time { get; set; }
    }
}
