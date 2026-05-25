using System;

namespace SHOP.CO.Domain.Entities
{
    public class CommerceRecord
    {
        public int RecordId { get; set; }
        public int? UserId { get; set; }
        public int? OrderId { get; set; }
        public int? ProductId { get; set; }
        public int? VariantId { get; set; }
        public string RecordType { get; set; } = null!; 
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? PaymentMethod { get; set; }
        public string? PaymentProvider { get; set; }
        public string? TransactionCode { get; set; }
        public decimal? Amount { get; set; }
        public string? DiscountType { get; set; }
        public decimal? DiscountValue { get; set; }
        public decimal? MaxDiscountAmount { get; set; }
        public decimal? MinOrderAmount { get; set; }
        public int? UsageLimit { get; set; }
        public int UsedCount { get; set; } 
        public DateTime? StartAt { get; set; }
        public DateTime? EndAt { get; set; }
        public string Status { get; set; } = null!; 
        public string? PayloadJson { get; set; }
        public DateTime CreatedAt { get; set; } 
        public DateTime? UpdatedAt { get; set; }

        public virtual User? User { get; set; }
        public virtual Order? Order { get; set; }
        public virtual Product? Product { get; set; }
        public virtual ProductVariant? ProductVariant { get; set; }
    }
}
