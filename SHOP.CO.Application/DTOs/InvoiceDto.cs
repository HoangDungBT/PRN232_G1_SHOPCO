using System;
using System.Collections.Generic;

namespace SHOP.CO.Application.DTOs
{
    public class InvoiceDto
    {
        public string InvoiceCode { get; set; } = null!;
        public string OrderCode { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public decimal SubtotalAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentStatus { get; set; } = null!;
        public string OrderStatus { get; set; } = null!;
        public string CustomerName { get; set; } = null!;
        public string? CustomerAddress { get; set; }
        public string? CustomerPhone { get; set; }
        public List<InvoiceItemDto> Items { get; set; } = new List<InvoiceItemDto>();
    }

    public class InvoiceItemDto
    {
        public string ProductName { get; set; } = null!;
        public string Sku { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Total { get; set; }
    }
}
