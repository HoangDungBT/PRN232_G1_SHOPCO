using System;
using System.Collections.Generic;

namespace SHOP.CO.Domain.Entities
{
    public class Product
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
        public DateTime CreatedAt { get; set; } 
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual Category Category { get; set; } = null!;
        public virtual ICollection<ProductVariant> ProductVariants { get; set; } = new List<ProductVariant>();
        public virtual ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();
    }
}
