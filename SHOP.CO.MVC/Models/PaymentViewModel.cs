namespace SHOP.CO.MVC.Models
{
    public class PaymentResponseViewModel
    {
        public bool Success { get; set; }
        public string? PaymentUrl { get; set; }
        public string? Message { get; set; }
        public string? OrderCode { get; set; }
        public string? PaymentStatus { get; set; }
    }

    public class PaymentIndexViewModel
    {
        public int OrderId { get; set; }
        public string OrderCode { get; set; } = null!;
        public decimal TotalAmount { get; set; }
        public string? OrderStatus { get; set; }
        public string? PaymentStatus { get; set; }
        public string? PaymentMethod { get; set; }
    }
}
