using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SHOP.CO.Infrastructure.Repositories
{
    public interface IUserRepository
    {
        Task<bool> EmailExistsAsync(string email);
        Task<User?> GetUserByEmailAsync(string email);
        Task<User?> GetUserByRefreshTokenAsync(string refreshToken);
        Task<User?> GetUserByIdAsync(int id);
        Task AddUserAsync(User user);
        Task UpdateUserAsync(User user);
        
        void UpdateUser(User user);
        void DeleteUser(User user);
        Task SaveChangesAsync();
    }

    public class UserRepository : IUserRepository
    {
        private readonly ShopCoDbContext _context;

        public UserRepository(ShopCoDbContext context)
        {
            _context = context;
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
           return await _context.Users.AnyAsync(u => u.Email == email);
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task AddUserAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task<User?> GetUserByRefreshTokenAsync(string refreshToken)
        {
            return await _context.Users.SingleOrDefaultAsync(u => u.RefreshToken == refreshToken);
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _context.Users
                .Include(u => u.UserAddresses)
                .FirstOrDefaultAsync(u => u.UserId == id);
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
