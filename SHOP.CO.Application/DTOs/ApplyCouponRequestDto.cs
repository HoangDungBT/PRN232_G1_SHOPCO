namespace SHOP.CO.Application.DTOs
{
    public class ApplyCouponRequestDto
    {
        public int UserId { get; set; }
        public string? CouponCode { get; set; }
    }
}
