using System.Threading.Tasks;
using SHOP.CO.Domain.Entities;

namespace SHOP.CO.Infrastructure.Repositories
{
    public interface ICommerceRecordRepository
    {
        Task<CommerceRecord?> GetCouponByCodeAsync(string code);
        Task SaveChangesAsync();
    }
}
