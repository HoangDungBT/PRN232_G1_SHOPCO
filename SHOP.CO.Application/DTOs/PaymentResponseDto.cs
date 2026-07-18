namespace SHOP.CO.Application.DTOs
{
    public class PaymentResponseDto
    {
        /// <summary>
        /// Whether the payment process call succeeded
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// VNPay redirect URL. Null for COD payments.
        /// </summary>
        public string? PaymentUrl { get; set; }

        /// <summary>
        /// Human-readable result message
        /// </summary>
        public string Message { get; set; } = null!;

        /// <summary>
        /// The order code (ORD-XXXXXX)
        /// </summary>
        public string? OrderCode { get; set; }

        /// <summary>
        /// Updated payment status: Unpaid | Paid | Failed
        /// </summary>
        public string? PaymentStatus { get; set; }
    }
}
