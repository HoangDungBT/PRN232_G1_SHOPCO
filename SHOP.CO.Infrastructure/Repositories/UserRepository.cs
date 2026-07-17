using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SHOP.CO.Application.Repositories;
using SHOP.CO.Domain.Entities;
using SHOP.CO.Infrastructure.Data;

namespace SHOP.CO.Infrastructure.Repositories
{
    public interface IUserRepository : IBaseRepository<User>
    {
        Task<bool> EmailExistsAsync(string email);
        Task<User?> GetUserByEmailAsync(string email);
        Task<User?> GetUserByRefreshTokenAsync(string refreshToken);
    }
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(ShopCoDbContext context) : base(context) { }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _dbSet.AnyAsync(u => u.Email == email);
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> GetUserByRefreshTokenAsync(string refreshToken)
        {
            return await _dbSet.SingleOrDefaultAsync(u => u.RefreshToken == refreshToken);
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _dbSet
                .Include(u => u.UserAddresses)
                .FirstOrDefaultAsync(u => u.UserId == id);
        }

        public async Task AddUserAsync(User user)
        {
            _dbSet.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateUserAsync(User user)
        {
            _dbSet.Update(user);
            await _context.SaveChangesAsync();
        }

        public void UpdateUser(User user) => _dbSet.Update(user);

        public void DeleteUser(User user)
        {
            user.Status = "Deleted"; // Soft Delete
            _dbSet.Update(user);
        }

        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
    }
}
