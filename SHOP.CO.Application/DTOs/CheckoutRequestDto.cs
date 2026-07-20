namespace SHOP.CO.Application.DTOs
{
    public class CheckoutRequestDto
    {
        public int UserId { get; set; } = 1;
        public string? CouponCode { get; set; }
        public int? AddressId { get; set; }
        public string? CustomerNote { get; set; }
        public string? PaymentMethod { get; set; }
        public string? OtpCode { get; set; }
    }
}
