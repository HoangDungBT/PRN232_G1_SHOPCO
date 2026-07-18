using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using SHOP.CO.Domain.Entities;
using SHOP.CO.Infrastructure.Persistence;

namespace SHOP.CO.Infrastructure.Repositories
{
    public class CommerceRecordRepository : ICommerceRecordRepository
    {
        private readonly ShopCoDbContext _context;

        public CommerceRecordRepository(ShopCoDbContext context)
        {
            _context = context;
        }

        public async Task<CommerceRecord?> GetCouponByCodeAsync(string code)
        {
            return await _context.CommerceRecords
                .FirstOrDefaultAsync(c => c.RecordType == "Voucher" && c.Code == code);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
