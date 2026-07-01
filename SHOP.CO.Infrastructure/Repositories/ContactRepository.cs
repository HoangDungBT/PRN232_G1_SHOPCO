using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SHOP.CO.Infrastructure.Repositories
{
    public class ContactRepository : IContactRepository
    {
        private readonly ShopCoDbContext _context;
        public ContactRepository(ShopCoDbContext context) => _context = context;

        public void AddContactLog(InteractionLog log)
        {
            _context.InteractionLogs.Add(log);
        }

        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
    }
}
