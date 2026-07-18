using System;

namespace SHOP.CO.MVC.Models
{
    public class OrderItemViewModel
    {
        public int OrderItemId { get; set; }
        public int OrderId { get; set; } 
        public int ProductId { get; set; } 
        public int VariantId { get; set; } 
        public string ProductNameSnapshot { get; set; } = null!; 
        public string SkuSnapshot { get; set; } = null!; 
        public string? SizeSnapshot { get; set; }
        public string? ColorSnapshot { get; set; }
        public string? ImageUrlSnapshot { get; set; }
        public decimal UnitPrice { get; set; } 
        public decimal? SalePrice { get; set; }
        public int Quantity { get; set; } 
        public decimal DiscountAmount { get; set; } 
        public decimal LineTotal { get; set; } 
        public string ReviewStatus { get; set; } = null!;
        public DateTime CreatedAt { get; set; } 

        public string DisplayImageUrl => GetProductImageUrl(ProductNameSnapshot);

        private string GetProductImageUrl(string productName)
        {
            if (string.IsNullOrWhiteSpace(productName)) return "/images/p2.jpg";
            var name = productName.ToLower();
            if (name.Contains("thun") || name.Contains("t-shirt")) return "/images/topsellingimg1.png";
            if (name.Contains("váy") || name.Contains("dress")) return "/images/newarrivalimg2.png";
            if (name.Contains("hoodie") || name.Contains("khoác")) return "/images/newarrivalimg3.png";
            if (name.Contains("jeans") || name.Contains("quần")) return "/images/p2.jpg";
            if (name.Contains("sơ mi") || name.Contains("shirt")) return "/images/topsellingimg4.png";
            return "/images/p2.jpg";
        }
    }
}
