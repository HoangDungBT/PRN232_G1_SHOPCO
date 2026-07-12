using System;

namespace SHOP.CO.Application.DTOs
{
    public class ProductImageDto
    {
        public int ImageId { get; set; }
        public int ProductId { get; set; }
        public int? VariantId { get; set; }
        public string ImageUrl { get; set; } = null!;
        public string? AltText { get; set; }
        public bool IsThumbnail { get; set; }
        public int SortOrder { get; set; }
    }
}
