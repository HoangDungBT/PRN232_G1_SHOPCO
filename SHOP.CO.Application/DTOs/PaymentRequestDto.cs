namespace SHOP.CO.Application.DTOs
{
    public class PaymentRequestDto
    {
        /// <summary>
        /// ID of the order to pay for
        /// </summary>
        public int OrderId { get; set; }

        /// <summary>
        /// Payment method: "COD" or "VNPay"
        /// </summary>
        public string PaymentMethod { get; set; } = null!;

        /// <summary>
        /// MVC return URL used by VNPay callback (optional for COD)
        /// </summary>
        public string? ReturnUrl { get; set; }
    }
}
