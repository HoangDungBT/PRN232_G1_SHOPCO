using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SHOP.CO.Application.DTOs
{
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

        [MinLength(1, ErrorMessage = "Sản phẩm phải có ít nhất 1 biến thể (Size/Màu)")]
        public List<CreateVariantDto> Variants { get; set; } = new List<CreateVariantDto>();
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

        public List<CreateVariantDto> Variants { get; set; } = new List<CreateVariantDto>();
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
}
