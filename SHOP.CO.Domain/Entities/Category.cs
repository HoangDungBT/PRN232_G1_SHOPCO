using System;
using System.Collections.Generic;

namespace SHOP.CO.Domain.Entities
{
    public class Category
    {
        public int CategoryId { get; set; } 
        public int? ParentCategoryId { get; set; } 
        public string CategoryName { get; set; } = null!; 
        public string Slug { get; set; } = null!; 
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public int SortOrder { get; set; } 
        public bool IsActive { get; set; } 
        public DateTime CreatedAt { get; set; } 
        public DateTime? UpdatedAt { get; set; }

        public virtual Category? ParentCategory { get; set; }
        public virtual ICollection<Category> SubCategories { get; set; } = new List<Category>();
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
