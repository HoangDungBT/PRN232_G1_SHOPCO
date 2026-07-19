namespace SHOP.CO.Application.DTOs
{
    public class ProductUiDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public string Image { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Category { get; set; } = null!;
        public decimal BasePrice { get; set; }
        public decimal? SalePrice { get; set; }
    }
}
