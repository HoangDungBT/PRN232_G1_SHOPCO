namespace SHOP.CO.Application.DTOs
{
    /// <summary>
    /// Data Transfer Object for adding an item to the shopping cart
    /// Used in API requests from client
    /// </summary>
    public class AddToCartRequestDto
    {
        /// <summary>
        /// Product variant ID (represents specific Size + Color combination)
        /// </summary>
        public int VariantId { get; set; }

        /// <summary>
        /// Quantity of items to add to cart
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Optional: Selected size preference
        /// </summary>
        public string? SelectedSize { get; set; }

        /// <summary>
        /// Optional: Selected color preference
        /// </summary>
        public string? SelectedColor { get; set; }
    }
}
