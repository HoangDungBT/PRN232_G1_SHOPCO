using SHOP.CO.Application.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SHOP.CO.Application.Services
{
    public interface IUserAdminService
    {
        IQueryable<UserDto> GetUserODataQuery();
        Task<ResultModel<bool>> UpdateUserStatusAsync(int userId, string newStatus);
        Task<ResultModel<bool>> UpdateUserRoleAsync(int userId, string newRole);
    }
    public class UserAdminService : IUserAdminService
    {
        private readonly IUserRepository _userRepo;

        public UserAdminService(IUserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        public IQueryable<UserDto> GetUserODataQuery()
        {
            return _userRepo.GetUsersAsQueryable().Select(
                u => new UserDto
                {
                    UserId = u.UserId,
                    FullName = u.FullName,
                    Email = u.Email,
                    Phone = u.Phone,
                    Role = u.Role,
                    Status = u.Status,
                    CreateAt = u.CreatedAt,
                    LastLoginAt = u.LastLoginAt,
                }
                );
        }

        public async Task<ResultModel<bool>> UpdateUserRoleAsync(int userId, string newRole)
        {
            try
            {
               var user = await _userRepo.GetUserByIdAsync( userId );
                if (user == null) { return ResultModel<bool>.Error("Lỗi không tìm thấy người dùng này", 404); }
                if (user.Role == "Admin") { ResultModel<bool>.Error("Không thể sửa đổi tài khoản này", 404); }

                user.Role = newRole;
                user.UpdatedAt = DateTime.UtcNow;
                await _userRepo.UpdateUserAsync( user );
                return ResultModel<bool>.Success(true, $"Đã phân quyền tài khoản thành: {newRole}");

            }
            catch (Exception ex)
            {
                return ResultModel<bool>.Exception(ex);
            }
        }

        public async Task<ResultModel<bool>> UpdateUserStatusAsync(int userId, string newStatus)
        {
            try
            {
                var user = await _userRepo.GetUserByIdAsync(userId);
                if (user == null) { return ResultModel<bool>.Error("Lỗi không tìm thấy người dùng này", 404); }
                if (user.Role == "Admin") { ResultModel<bool>.Error("Không thể sửa đổi tài khoản này", 404); }

                user.Status = newStatus;
                user.UpdatedAt = DateTime.UtcNow;
                await _userRepo.UpdateUserAsync(user);
                return ResultModel<bool>.Success(true, $"Đã cập nhật trạng thái thành: {newStatus}");

            }
            catch (Exception ex)
            {
                return ResultModel<bool>.Exception(ex);
            }
        }
    }
}
