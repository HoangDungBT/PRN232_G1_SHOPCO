using SHOP.CO.Domain.Entities;

namespace SHOP.CO.Domain.Repositories
{
    public interface IProductUiRepository
    {
        Task<IEnumerable<ProductUiEntity>> GetAllAsync();
        Task<ProductUiEntity?> GetByIdAsync(int id);
    }
}
