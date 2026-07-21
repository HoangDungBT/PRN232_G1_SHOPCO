namespace SHOP.CO.MVC.Models
{
    public class ProductVM
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public string Image { get; set; }

        public string Description { get; set; }

        public string Category { get; set; }

        public decimal BasePrice { get; set; }

        public decimal? SalePrice { get; set; }

        public decimal AverageRating { get; set; }

        public int ReviewCount { get; set; }
    }
}