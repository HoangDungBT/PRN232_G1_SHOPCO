using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SHOP.CO.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ShopCoDbContext _context;
        public UserRepository(ShopCoDbContext context) => _context = context;

        public async Task<User> GetUserByIdAsync(int userId)
        {
            return await _context.Users
                .Include(u => u.UserAddresses)
                .FirstOrDefaultAsync(u => u.UserId == userId);
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public void UpdateUser(User user) => _context.Users.Update(user);

        public void DeleteUser(User user)
        {
            user.Status = "Deleted"; // Soft Delete
            _context.Users.Update(user);
        }

        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
    }
}
