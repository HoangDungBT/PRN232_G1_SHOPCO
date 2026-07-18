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
                Image = "https://via.placeholder.com/600x800",
                Description = "Premium cotton t-shirt.",
                Category = "T-Shirts"
            },
            new ProductUiEntity
            {
                Id = 2,
                Name = "Jeans",
                Price = 59,
                Image = "https://via.placeholder.com/600x800",
                Description = "Modern slim fit jeans.",
                Category = "Jeans"
            },
            new ProductUiEntity
            {
                Id = 3,
                Name = "Hoodie",
                Price = 99,
                Image = "https://via.placeholder.com/600x800",
                Description = "Warm fashion hoodie.",
                Category = "Hoodies"
            },
            new ProductUiEntity
            {
                Id = 4,
                Name = "Jacket",
                Price = 120,
                Image = "https://via.placeholder.com/600x800",
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
