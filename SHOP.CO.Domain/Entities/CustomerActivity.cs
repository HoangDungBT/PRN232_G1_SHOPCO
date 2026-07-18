using System;

namespace SHOP.CO.Domain.Entities
{
    public class CustomerActivity
    {
        public int ActivityId { get; set; }
        public int? UserId { get; set; }
        public int? ProductId { get; set; }
        public int? VariantId { get; set; }
        public int? OrderItemId { get; set; }
        public string ActivityType { get; set; } = null!; 
        public int? Rating { get; set; }
        public string? Comment { get; set; }
        public string? Keyword { get; set; }
        public string? InputJson { get; set; }
        public string? ResultJson { get; set; }
        public string? SessionId { get; set; }
        public string? IpAddress { get; set; }
        public bool IsActive { get; set; } 
        public DateTime CreatedAt { get; set; } 
        public DateTime? UpdatedAt { get; set; }

        public virtual User? User { get; set; }
        public virtual Product? Product { get; set; }
        public virtual ProductVariant? ProductVariant { get; set; }
        public virtual OrderItem? OrderItem { get; set; }
    }
}
