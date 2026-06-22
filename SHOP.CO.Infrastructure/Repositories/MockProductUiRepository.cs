using SHOP.CO.Domain.Entities;
using SHOP.CO.Domain.Repositories;

namespace SHOP.CO.Infrastructure.Repositories
{
    public class MockProductUiRepository : IProductUiRepository
    {
        private static readonly List<ProductUiEntity> _mockProducts = new()
        {
            new ProductUiEntity
            {
                Id = 1,
                Name = "T-Shirt",
                Price = 29,
                Image = "/images/newarrivalimg1.png",
                Description = "Premium cotton t-shirt.",
                Category = "T-Shirts"
            },
            new ProductUiEntity
            {
                Id = 2,
                Name = "Jeans",
                Price = 59,
                Image = "/images/newarrivalimg2.png",
                Description = "Modern slim fit jeans.",
                Category = "Jeans"
            },
            new ProductUiEntity
            {
                Id = 3,
                Name = "Hoodie",
                Price = 99,
                Image = "/images/newarrivalimg3.png",
                Description = "Warm fashion hoodie.",
                Category = "Hoodies"
            },
            new ProductUiEntity
            {
                Id = 4,
                Name = "Jacket",
                Price = 120,
                Image = "/images/newarrivalimg4.png",
                Description = "Luxury winter jacket.",
                Category = "Jackets"
            }
        };

        public Task<IEnumerable<ProductUiEntity>> GetAllAsync()
        {
            return Task.FromResult<IEnumerable<ProductUiEntity>>(_mockProducts);
        }

        public Task<ProductUiEntity?> GetByIdAsync(int id)
        {
            var product = _mockProducts.FirstOrDefault(x => x.Id == id);
            return Task.FromResult(product);
        }
    }
}
