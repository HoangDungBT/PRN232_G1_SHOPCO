using System;

namespace SHOP.CO.Domain.Entities
{
    public class CartItem
    {
        public int CartItemId { get; set; }
        public int? UserId { get; set; }
        public string? SessionId { get; set; }
        public int VariantId { get; set; } 
        public int Quantity { get; set; } 
        public decimal UnitPrice { get; set; } 
        public string? SelectedSize { get; set; }
        public string? SelectedColor { get; set; }
        public bool IsSelected { get; set; } 
        public DateTime CreatedAt { get; set; } 
        public DateTime? UpdatedAt { get; set; }

        public virtual User? User { get; set; }
        public virtual ProductVariant ProductVariant { get; set; } = null!;
    }
}
