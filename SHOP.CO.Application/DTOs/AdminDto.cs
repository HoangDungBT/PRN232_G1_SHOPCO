using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SHOP.CO.Application.DTOs
{
    public class DashboardSummaryDto
    {
        public int TotalUsers { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public int LowStockProducts { get; set; }
    }

    public class RevenueByDayDto
    {
        public string Date { get; set; } = string.Empty;
        public decimal Revenue { get; set; }
    }
    public static class LogTypes
    {
        public const string StockImport = "StockImport"; // Nhập kho
        public const string StockExport = "StockExport"; // Xuất kho (hư hỏng, hủy)
        public const string StockAdjustment = "StockAdjustment"; // Điều chỉnh kho (cân lại kho)
        public const string SaleDeduction = "SaleDeduction"; // Trừ kho khi có đơn hàng
    }

    public class StockAdjustmentDto
    {
        public int VariantId { get; set; }
        public int QuantityChange { get; set; } 
        public string Reason { get; set; } = string.Empty;
    }

    #region CategoryAdmin

    public class SaveCategoryRequestDto
    {
        public int? ParentCategoryId { get; set; }

        [Required(ErrorMessage = "Tên danh mục không được để trống")]
        public string CategoryName { get; set; } = string.Empty;

        public string? Description { get; set; }
        public string? ImageUrl { get; set; }

        [Range(0, int.MaxValue)]
        public int SortOrder { get; set; } = 0;

        public bool IsActive { get; set; } = true;
    }

    // DTO Trả về danh sách (Cho OData) và Chi tiết
    public class CategoryDto
    {
        public int CategoryId { get; set; }
        public int? ParentCategoryId { get; set; }
        public string? ParentCategoryName { get; set; } // Hiển thị tên danh mục cha cho dễ nhìn
        public string CategoryName { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int SortOrder { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
    #endregion


    #region OrderDto
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
    #endregion

    #region ProductDto
    public class ProductDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public string Slug { get; set; } = null!;
        public decimal BasePrice { get; set; }
        public decimal? SalePrice { get; set; }
        public string? CategoryName { get; set; }
        public bool IsActive { get; set; }
        public string? ThumbnailUrl { get; set; }
        public bool HasLowStock { get; set; }

    }
    public class BulkUpdateStatusDto
    {
        public List<int> ProductIds { get; set; } = new List<int>();
        public bool IsActive { get; set; }
    }

    public class BulkUpdateFeaturedDto
    {
        public List<int> ProductIds { get; set; } = new List<int>();
        public bool IsFeatured { get; set; }
    }

    // DTO Hứng dữ liệu Thêm mới sản phẩm
    public class CreateProductRequestDto
    {
        [Required(ErrorMessage = "Danh mục không được để trống")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Tên sản phẩm không được để trống")]
        public string ProductName { get; set; } = string.Empty;

        public string? Brand { get; set; }
        public string? Description { get; set; }
        public string? Material { get; set; }
        public string? GenderTarget { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Giá gốc phải lớn hơn 0")]
        public decimal BasePrice { get; set; }
        public decimal? SalePrice { get; set; }

        public bool IsFeatured { get; set; }
        public bool IsActive { get; set; } = true;
        public List<string>? ImageUrls { get; set; } = new List<string>();

        [MinLength(1, ErrorMessage = "Sản phẩm phải có ít nhất 1 biến thể (Size/Màu)")]
        public List<CreateVariantDto> Variants { get; set; } = new List<CreateVariantDto>();
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (BasePrice % 1000 != 0)
                yield return new ValidationResult("Giá gốc phải chẵn hàng nghìn (VD: 150000)!", new[] { nameof(BasePrice) });

            if (SalePrice.HasValue && SalePrice.Value % 1000 != 0)
                yield return new ValidationResult("Giá khuyến mãi phải chẵn hàng nghìn!", new[] { nameof(SalePrice) });
        }
    }

    public class CreateVariantDto
    {
        [Required(ErrorMessage = "Mã SKU là bắt buộc")]
        public string Sku { get; set; } = string.Empty;
        public string? Size { get; set; }
        public string? Color { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "Giá thêm phải lớn hơn 0")]
        public decimal ExtraPrice { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Tồn kho không hợp lệ")]
        public int StockQuantity { get; set; }
        public bool IsActive { get; set; } = true;
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (ExtraPrice % 1000 != 0)
            {
                yield return new ValidationResult("Giá thêm của biến thể phải chẵn hàng nghìn (VD: 1000, 50000)!", new[] { nameof(ExtraPrice) });
            }
        }
    }

    // DTO Hứng dữ liệu Cập nhật sản phẩm (Chỉ cập nhật thông tin cơ bản)
    public class UpdateProductRequestDto
    {
        [Required] public int CategoryId { get; set; }
        [Required] public string ProductName { get; set; } = string.Empty;
        public string? Brand { get; set; }
        public string? Description { get; set; }
        public string? Material { get; set; }
        public string? GenderTarget { get; set; }
        [Range(0, double.MaxValue)] public decimal BasePrice { get; set; }
        public decimal? SalePrice { get; set; }
        public bool IsFeatured { get; set; }
        public bool IsBestSeller { get; set; }
        public bool IsNewArrival { get; set; }
        public bool IsActive { get; set; }
        public List<string>? ImageUrls { get; set; } = new List<string>();

        [MinLength(1, ErrorMessage = "Sản phẩm phải có ít nhất 1 biến thể (Size/Màu)")]
        public List<CreateVariantDto> Variants { get; set; } = new List<CreateVariantDto>();
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (BasePrice % 1000 != 0)
                yield return new ValidationResult("Giá gốc phải chẵn hàng nghìn (VD: 150000)!", new[] { nameof(BasePrice) });

            if (SalePrice.HasValue && SalePrice.Value % 1000 != 0)
                yield return new ValidationResult("Giá khuyến mãi phải chẵn hàng nghìn!", new[] { nameof(SalePrice) });
        }

    }

    // DTO Trả về chi tiết Sản phẩm để binding lên Form Sửa
    public class ProductDetailAdminDto
    {
        public int ProductId { get; set; }
        public int CategoryId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string? Brand { get; set; }
        public string? Description { get; set; }
        public string? Material { get; set; }
        public string? GenderTarget { get; set; }
        public decimal BasePrice { get; set; }
        public decimal? SalePrice { get; set; }
        public bool IsFeatured { get; set; }
        public bool IsBestSeller { get; set; }
        public bool IsNewArrival { get; set; }
        public bool IsActive { get; set; }
        public List<ProductImageDto> Images { get; set; } = new List<ProductImageDto>();
        public List<CreateVariantDto> Variants { get; set; } = new List<CreateVariantDto>();
    }
    public class ProductImageDto
    {
        public int ImageId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public bool IsThumbnail { get; set; }
    }
    //DTO chứa toàn bộ dữ liệu cấu hình cho Form
    public class ProductFormAttributesDto
    {
        // Category lấy từ Database
        public List<CategorySimpleDto> Categories { get; set; } = new List<CategorySimpleDto>();
        // Các thuộc tính còn lại lấy từ Domain/Shared
        public List<string> Brands { get; set; } = new List<string>();
        public List<string> Materials { get; set; } = new List<string>();
        public List<string> Sizes { get; set; } = new List<string>();
        public List<string> Colors { get; set; } = new List<string>();
    }

    // DTO rút gọn cho Category để nhét vào ComboBox
    public class CategorySimpleDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
    #endregion

    #region UserDto
    public class UserDto
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string Role { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreateAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
    }
    public class UpdateUserFieldDto
    {
        public string Value { get; set; } = string.Empty;
    }
    #endregion
}
