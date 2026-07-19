using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SHOP.CO.Infrastructure.Repositories
{
    public class CommerceRecordRepository : BaseRepository<CommerceRecord>, ICommerceRecordRepository
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
    }
}
