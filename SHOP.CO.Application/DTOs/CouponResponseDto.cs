namespace SHOP.CO.Application.DTOs
{
    public class CouponResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public decimal DiscountAmount { get; set; }
        public decimal FinalAmount { get; set; }
    }
}
