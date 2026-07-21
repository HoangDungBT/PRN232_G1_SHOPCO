namespace SHOP.CO.Application.DTOs
{
    /// <summary>
    /// Data Transfer Object for a single cart item
    /// Used in API responses to display cart items to client
    /// </summary>
    public class CartItemDto
    {
        /// <summary>
        /// Unique identifier for the cart item
        /// </summary>
        public int CartItemId { get; set; }

        /// <summary>
        /// Product variant ID
        /// </summary>
        public int VariantId { get; set; }

        /// <summary>
        /// Product ID for linking back to product details
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// Product name for display
        /// </summary>
        public string ProductName { get; set; } = null!;

        /// <summary>
        /// Product main image URL for display
        /// </summary>
        public string? ImageUrl { get; set; }

        /// <summary>
        /// Product SKU (Stock Keeping Unit)
        /// </summary>
        public string Sku { get; set; } = null!;

        /// <summary>
        /// Size variant (e.g., "M", "L", "XL")
        /// </summary>
        public string? Size { get; set; }

        /// <summary>
        /// Color variant name (e.g., "Red", "Blue")
        /// </summary>
        public string? Color { get; set; }

        /// <summary>
        /// Color hex code for visual display (e.g., "#FF0000")
        /// </summary>
        public string? ColorHex { get; set; }

        /// <summary>
        /// Quantity of this variant in cart
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Unit price of this variant (captured at time of adding to cart)
        /// </summary>
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// Total price for this item (Quantity * UnitPrice)
        /// </summary>
        public decimal TotalPrice => Quantity * UnitPrice;

        /// <summary>
        /// Selected size preference by user
        /// </summary>
        public string? SelectedSize { get; set; }

        /// <summary>
        /// Selected color preference by user
        /// </summary>
        public string? SelectedColor { get; set; }

        /// <summary>
        /// Whether this item is selected for checkout
        /// </summary>
        public bool IsSelected { get; set; }

        /// <summary>
        /// Current stock quantity available for this variant
        /// </summary>
        public int AvailableStock { get; set; }

        /// <summary>
        /// Timestamp when this item was added to cart
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Timestamp when this item was last updated
        /// </summary>
        public DateTime? UpdatedAt { get; set; }
    }
}
