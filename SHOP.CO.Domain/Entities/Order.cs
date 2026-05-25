using System;
using System.Collections.Generic;

namespace SHOP.CO.Domain.Entities
{
    public class Order
    {
        public int OrderId { get; set; }
        public int UserId { get; set; } 
        public string OrderCode { get; set; } = null!; 
        public int? AddressId { get; set; }
        public string ReceiverName { get; set; } = null!; 
        public string ReceiverPhone { get; set; } = null!; 
        public string ShippingAddressText { get; set; } = null!; 
        public string OrderStatus { get; set; } = null!; 
        public string PaymentStatus { get; set; } = null!; 
        public string ShippingStatus { get; set; } = null!; 
        public decimal SubtotalAmount { get; set; } 
        public decimal DiscountAmount { get; set; } 
        public decimal ShippingFee { get; set; } 
        public decimal TotalAmount { get; set; } 
        public string? CustomerNote { get; set; }
        public string? StaffNote { get; set; }
        public string? CancelReason { get; set; }
        public DateTime CreatedAt { get; set; } 
        public DateTime? UpdatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public DateTime? CanceledAt { get; set; }

        public virtual User User { get; set; } = null!;
        public virtual UserAddress? UserAddress { get; set; }
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public virtual ICollection<CommerceRecord> CommerceRecords { get; set; } = new List<CommerceRecord>();
    }
}
