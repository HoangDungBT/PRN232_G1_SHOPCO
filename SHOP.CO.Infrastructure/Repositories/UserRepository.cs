namespace SHOP.CO.Infrastructure.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(ShopCoDbContext context) : base(context) { }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _dbSet.AnyAsync(u => u.Email == email);
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _dbSet.SingleOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> GetUserByRefreshTokenAsync(string refreshToken)
        {
            return await _dbSet.SingleOrDefaultAsync(u => u.RefreshToken == refreshToken);
        }
    }
}
//    public interface IUserRepository
//    {
//        Task<bool> EmailExistsAsync(string email);
//        Task<User?> GetUserByEmailAsync(string email);
//        Task<User?> GetUserByRefreshTokenAsync(string refreshToken);
//        IQueryable<User> GetUsersAsQueryable();
//        Task<User?> GetUserByIdAsync(int id);
//        Task AddUserAsync(User user);
//        Task UpdateUserAsync(User user);
//    }

//    public class UserRepository : IUserRepository
//    {
//        private readonly ShopCoDbContext _context;

//        public UserRepository(ShopCoDbContext context)
//        {
//            _context = context;
//        }

//        public async Task<bool> EmailExistsAsync(string email)
//        {
//           return await _context.Users.AnyAsync(u => u.Email == email);
//        }

//        public async Task<User?> GetUserByEmailAsync(string email)
//        {
//            return await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
//        }
//        public IQueryable<User> GetUsersAsQueryable()
//        {
//            return _context.Users.AsQueryable();
//        }
//        public async Task<User?> GetUserByIdAsync(int id)
//        {
//            return await _context.Users.FindAsync(id);
//        }
//        public async Task AddUserAsync(User user)
//        {
//            _context.Users.Add(user);
//            await _context.SaveChangesAsync();
//        }


//        public async Task UpdateUserAsync(User user)
//        {
//            _context.Users.Update(user);
//            await _context.SaveChangesAsync();
//        }

//        public async Task<User?> GetUserByRefreshTokenAsync(string refreshToken)
//        {
//            return await _context.Users.SingleOrDefaultAsync(u => u.RefreshToken == refreshToken);
//        }

//    }
//}
