namespace SHOP.CO.Application.DTOs
{
    /// <summary>
    /// Request DTO for updating the quantity of a cart item.
    /// </summary>
    public class UpdateCartQuantityRequest
    {
        /// <summary>
        /// The new quantity for the cart item.
        /// </summary>
        public int Quantity { get; set; }
    }
}
