using System;

namespace SHOP.CO.Application.DTOs
{
    public class ProductVariantDto
    {
        public int VariantId { get; set; }
        public int ProductId { get; set; }
        public string Sku { get; set; } = null!;
        public string? Size { get; set; }
        public string? Color { get; set; }
        public string? ColorHex { get; set; }
        public decimal ExtraPrice { get; set; }
        public decimal? OriginalPrice { get; set; }
        public int StockQuantity { get; set; }
        public int LowStockThreshold { get; set; }
        public string? Barcode { get; set; }
        public int? WeightGram { get; set; }
    }
}
