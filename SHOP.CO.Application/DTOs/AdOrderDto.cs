using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SHOP.CO.Application.DTOs
{
    // DTO Dùng cho OData (Hiển thị bảng)
    public class OrderDto
    {
        public int OrderId { get; set; }
        public string OrderCode { get; set; } = string.Empty;
        public string ReceiverName { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public string OrderStatus { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    // DTO Chi tiết Đơn hàng để binding lên Form Sửa
    public class OrderDetailAdminDto : OrderDto
    {
        public string ReceiverPhone { get; set; } = string.Empty;
        public string ShippingAddressText { get; set; } = string.Empty;
        public decimal SubtotalAmount { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal DiscountAmount { get; set; }
        public string? CustomerNote { get; set; }
        public string? StaffNote { get; set; }
        public string? CancelReason { get; set; }

        public List<OrderItemAdminDto> Items { get; set; } = new List<OrderItemAdminDto>();
    }

    // DTO Chi tiết sản phẩm trong đơn hàng
    public class OrderItemAdminDto
    {
        public int VariantId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string Sku { get; set; } = string.Empty;
        public string? Size { get; set; }
        public string? Color { get; set; }
        public string? ImageUrl { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal SalePrice { get; set; }
        public decimal LineTotal { get; set; }
    }

    // DTO Cập nhật trạng thái
    public class UpdateOrderStatusDto
    {
        public string Status { get; set; } = string.Empty;
        public string? CancelReason { get; set; } // Bắt buộc nếu Status = "Canceled"
    }
}
