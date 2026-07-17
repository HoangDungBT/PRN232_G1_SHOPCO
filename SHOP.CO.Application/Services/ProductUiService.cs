using SHOP.CO.Application.DTOs;
using SHOP.CO.Domain.Repositories;

namespace SHOP.CO.Application.Services
{
    public class ProductUiService : IProductUiService
    {
        private readonly IProductUiRepository _repository;

        public ProductUiService(IProductUiRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<ProductUiDto>> GetUiProductsAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(e => new ProductUiDto
            {
                Id = e.Id,
                Name = e.Name,
                Price = e.Price,
                Image = e.Image,
                Description = e.Description,
                Category = e.Category
            });
        }

        public async Task<ProductUiDto?> GetUiProductByIdAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return null;

            return new ProductUiDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Price = entity.Price,
                Image = entity.Image,
                Description = entity.Description,
                Category = entity.Category
            };
        }
    }
}
