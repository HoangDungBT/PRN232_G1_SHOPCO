using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SHOP.CO.Infrastructure.Repositories
{
    public class CommerceRecordRepository : BaseRepository<CommerceRecord>, SHOP.CO.Application.Repositories.ICommerceRecordRepository, SHOP.CO.Infrastructure.Repositories.ICommerceRecordRepository
    {
        public CommerceRecordRepository(ShopCoDbContext context) : base(context) { }

        public IQueryable<CommerceRecord> GetVouchersAsQueryable()
        {
            // Chỉ lấy những dòng đóng vai trò là Voucher hệ thống
            return _dbSet.Where(c => c.RecordType == "Voucher").AsQueryable();
        }

        public async Task<bool> IsVoucherCodeExistsAsync(string code, int? excludeId = null)
        {
            var query = _dbSet.Where(c => c.RecordType == "Voucher" && c.Code == code);

            if (excludeId.HasValue)
            {
                query = query.Where(c => c.RecordId != excludeId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<CommerceRecord?> GetCouponByCodeAsync(string code)
        {
            return await _dbSet.FirstOrDefaultAsync(c => c.RecordType == "Voucher" && c.Code == code);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
