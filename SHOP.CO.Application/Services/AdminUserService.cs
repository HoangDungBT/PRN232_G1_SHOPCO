

namespace SHOP.CO.Application.Services
{
    public interface IAdminUserService
    {
        IQueryable<UserDto> GetUserODataQuery();
        Task<ResultModel<bool>> UpdateUserStatusAsync(int userId, string newStatus);
        Task<ResultModel<bool>> UpdateUserRoleAsync(int userId, string newRole);
    }

    public class AdminUserService : IAdminUserService
    {
        private readonly IUserRepository _userRepo;
        private readonly IMapper _mapper; // 🟢 Inject AutoMapper

        public AdminUserService(IUserRepository userRepo, IMapper mapper)
        {
            _userRepo = userRepo;
            _mapper = mapper;
        }

        public IQueryable<UserDto> GetUserODataQuery()
        {
            // 🟢 TỰ ĐỘNG MAP
            return _userRepo.GetQueryable()
                            .ProjectTo<UserDto>(_mapper.ConfigurationProvider);
        }

        public async Task<ResultModel<bool>> UpdateUserRoleAsync(int userId, string newRole)
        {
            try
            {
                var user = await _userRepo.GetByIdAsync(userId); // BaseRepo
                if (user == null) return ResultModel<bool>.Error("Không tìm thấy người dùng này", 404);
                if (user.Role == "Admin") return ResultModel<bool>.Error("Không thể sửa đổi tài khoản này", 400);

                user.Role = newRole;
                user.UpdatedAt = DateTime.UtcNow;
                await _userRepo.UpdateAsync(user); // BaseRepo
                return ResultModel<bool>.Success(true, $"Đã phân quyền tài khoản thành: {newRole}");
            }
            catch (Exception ex) { return ResultModel<bool>.Exception(ex); }
        }

        public async Task<ResultModel<bool>> UpdateUserStatusAsync(int userId, string newStatus)
        {
            try
            {
                var user = await _userRepo.GetByIdAsync(userId);
                if (user == null) return ResultModel<bool>.Error("Không tìm thấy người dùng này", 404);
                if (user.Role == "Admin") return ResultModel<bool>.Error("Không thể sửa đổi tài khoản này", 400);

                user.Status = newStatus;
                user.UpdatedAt = DateTime.UtcNow;
                await _userRepo.UpdateAsync(user);
                return ResultModel<bool>.Success(true, $"Đã cập nhật trạng thái thành: {newStatus}");
            }
            catch (Exception ex) { return ResultModel<bool>.Exception(ex); }
        }
    }
}