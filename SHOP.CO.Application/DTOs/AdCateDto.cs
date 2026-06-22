using System.ComponentModel.DataAnnotations;


namespace SHOP.CO.Application.DTOs
{
    // DTO Hứng dữ liệu Thêm mới/Cập nhật (Dùng chung cho cả Create và Update để tiết kiệm code)
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
}
