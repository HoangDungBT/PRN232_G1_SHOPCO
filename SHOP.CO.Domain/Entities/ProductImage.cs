using System;

namespace SHOP.CO.Domain.Entities
{
    public class ProductImage
    {
        public int ImageId { get; set; }
        public int ProductId { get; set; } 
        public int? VariantId { get; set; }
        public string ImageUrl { get; set; } = null!; 
        public string? AltText { get; set; }
        public bool IsThumbnail { get; set; } 
        public int SortOrder { get; set; } 
        public string? FileName { get; set; }
        public long? FileSize { get; set; }
        public string? ContentType { get; set; }
        public string? DominantColorHex { get; set; }
        public string? ColorAnalysisJson { get; set; }
        public DateTime CreatedAt { get; set; } 

        public virtual Product Product { get; set; } = null!;
        public virtual ProductVariant? ProductVariant { get; set; }
    }
}
