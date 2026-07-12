using System;
using System.Collections.Generic;

namespace SHOP.CO.Application.DTOs
{
    public class ProductDto
    {
        public int ProductId { get; set; }
        public int CategoryId { get; set; }
        public string ProductName { get; set; } = null!;
        public string Slug { get; set; } = null!;
        public string? Brand { get; set; }
        public string? Description { get; set; }
        public string? Material { get; set; }
        public string? GenderTarget { get; set; }
        public decimal BasePrice { get; set; }
        public decimal? SalePrice { get; set; }
        public decimal AverageRating { get; set; }
        public int ReviewCount { get; set; }
        public int ViewCount { get; set; }
        public bool IsFeatured { get; set; }
        public bool IsBestSeller { get; set; }
        public bool IsNewArrival { get; set; }
        public bool IsActive { get; set; }
        public string? CategoryName { get; set; }
        public string? ThumbnailUrl { get; set; }

        public virtual List<ProductVariantDto> Variants { get; set; } = new List<ProductVariantDto>();
        public virtual List<ProductImageDto> Images { get; set; } = new List<ProductImageDto>();
    }
}
