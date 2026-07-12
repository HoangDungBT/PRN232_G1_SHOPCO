using System.Text.Json.Serialization;

namespace SHOP.CO.MVC.Models
{
    /// <summary>
    /// Generic API response wrapper used by SHOP.CO.API
    /// </summary>
    public class CartApiResponse<T>
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }

        [JsonPropertyName("data")]
        public T? Data { get; set; }
    }
    /// <summary>
    /// View model representing a cart item for the MVC view
    /// </summary>
    public class CartItemViewModel
    {
        public int CartItemId { get; set; }
        public int VariantId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Subtotal => Quantity * UnitPrice;
    }

    /// <summary>
    /// View model representing the whole cart
    /// </summary>
    public class CartViewModel
    {
        public int UserId { get; set; }
        public List<CartItemViewModel> Items { get; set; } = new List<CartItemViewModel>();
        public decimal TotalAmount => Items.Sum(i => i.Subtotal);
    }
}
