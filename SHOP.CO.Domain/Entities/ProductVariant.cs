using System;
using System.Collections.Generic;

namespace SHOP.CO.Domain.Entities
{
    public class ProductVariant
    {
        public int VariantId { get; set; }
        public int ProductId { get; set; } 
        public string Sku { get; set; } = null!; 
        public string? Size { get; set; }
        public string? Color { get; set; }
        public string? ColorHex { get; set; }
        public decimal ExtraPrice { get; set; } 
        public decimal? OriginalPrice { get; set; }
        public int StockQuantity { get; set; } 
        public int LowStockThreshold { get; set; } 
        public string? Barcode { get; set; }
        public int? WeightGram { get; set; }
        public bool IsActive { get; set; } 
        public DateTime CreatedAt { get; set; } 
        public DateTime? UpdatedAt { get; set; }

        public virtual Product Product { get; set; } = null!;
        public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    }
}
