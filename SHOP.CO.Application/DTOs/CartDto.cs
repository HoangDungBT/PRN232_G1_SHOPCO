namespace SHOP.CO.Application.DTOs
{
    /// <summary>
    /// Data Transfer Object for the complete shopping cart
    /// Used in API responses to display entire cart to client
    /// </summary>
    public class CartDto
    {
        /// <summary>
        /// User ID who owns this cart
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// List of all cart items
        /// </summary>
        public List<CartItemDto> Items { get; set; } = new List<CartItemDto>();

        /// <summary>
        /// Total number of items in cart (sum of all quantities)
        /// </summary>
        public int TotalItems => Items.Sum(i => i.Quantity);

        /// <summary>
        /// Subtotal amount (sum of all item total prices)
        /// </summary>
        public decimal SubtotalAmount => Items.Sum(i => i.TotalPrice);

        /// <summary>
        /// Number of distinct products in cart
        /// </summary>
        public int ItemCount => Items.Count;

        /// <summary>
        /// Timestamp when the cart was last updated
        /// </summary>
        public DateTime LastUpdated { get; set; }

        public decimal ShippingFee { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal FinalAmount { get; set; }
        public string? CouponCode { get; set; }
        public int? AddressId { get; set; }
        public string? AddressText { get; set; }
    }
}
