using System;

namespace SHOP.CO.Application.DTOs
{
    public class ShippingFeeRequestDto
    {
        public int UserId { get; set; }
        public int AddressId { get; set; }
        public decimal Subtotal { get; set; }
    }
}
