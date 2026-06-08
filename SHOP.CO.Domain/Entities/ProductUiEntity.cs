namespace SHOP.CO.Domain.Entities
{
    public class ProductUiEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public string Image { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Category { get; set; } = null!;
    }
}
