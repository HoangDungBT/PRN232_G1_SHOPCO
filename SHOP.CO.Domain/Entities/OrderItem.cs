using System;

namespace SHOP.CO.Domain.Entities
{
    public class OrderItem
    {
        public int OrderItemId { get; set; }
        public int OrderId { get; set; } 
        public int ProductId { get; set; } 
        public int VariantId { get; set; } 
        public string ProductNameSnapshot { get; set; } = null!; 
        public string SkuSnapshot { get; set; } = null!; 
        public string? SizeSnapshot { get; set; }
        public string? ColorSnapshot { get; set; }
        public string? ImageUrlSnapshot { get; set; }

        public decimal UnitPrice { get; set; } 
        public decimal? SalePrice { get; set; }
        public int Quantity { get; set; } 

        public decimal DiscountAmount { get; set; } 
        public decimal LineTotal { get; set; } 
        public string ReviewStatus { get; set; } = null!; 
        public DateTime CreatedAt { get; set; } 
        public DateTime? UpdatedAt { get; set; }

        public virtual Order Order { get; set; } = null!;
        public virtual Product Product { get; set; } = null!;
        public virtual ProductVariant ProductVariant { get; set; } = null!;
    }
}
