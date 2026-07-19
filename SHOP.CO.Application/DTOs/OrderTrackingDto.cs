using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SHOP.CO.Application.DTOs
{
    public class OrderTrackingDto
    {
        public int OrderId { get; set; }

        public string OrderCode { get; set; } = null!;

        public string OrderStatus { get; set; } = null!;

        public string PaymentStatus { get; set; } = null!;

        public decimal TotalAmount { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? CompletedAt { get; set; }

        [JsonPropertyName("statusHistory")]
        public List<OrderTrackingStepDto> Steps { get; set; } = new List<OrderTrackingStepDto>();
    }
}
