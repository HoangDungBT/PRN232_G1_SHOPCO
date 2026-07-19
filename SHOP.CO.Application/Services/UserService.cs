using SHOP.CO.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SHOP.CO.Application.Services
{
    public interface IUserService
    {
        Task<UserProfileDto> GetProfileAsync(int userId);
        Task<bool> UpdateProfileAsync(int userId, UserProfileDto dto);
        Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto dto);
        Task<bool> DeleteAccountAsync(int userId);
        Task<bool> ToggleNewsletterAsync(int userId, bool subscribe);
    }

    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher<User> _passwordHasher;

        public UserService(IUserRepository userRepository, IPasswordHasher<User> passwordHasher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<UserProfileDto> GetProfileAsync(int userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null) return null!;

            return new UserProfileDto
            {
                UserId = user.UserId,
                FullName = user.FullName,
                Email = user.Email,
                Phone = user.Phone,
                DateOfBirth = user.DateOfBirth,
                Gender = user.Gender,
                PreferredSize = user.PreferredSize,
                PreferredStyle = user.PreferredStyle,
                IsNewsletterSubscribed = user.IsNewsletterSubscribed
            };
        }

        public async Task<bool> UpdateProfileAsync(int userId, UserProfileDto dto)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null) return false;

            user.FullName = dto.FullName;
            user.Phone = dto.Phone;
            user.DateOfBirth = dto.DateOfBirth;
            user.Gender = dto.Gender;
            user.PreferredSize = dto.PreferredSize;
            user.PreferredStyle = dto.PreferredStyle;
            user.UpdatedAt = DateTime.UtcNow;

            _userRepository.UpdateUser(user);
            await _userRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto dto)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null) return false;

            // Kiểm tra mật khẩu cũ
            var verifyResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.CurrentPassword);
            if (verifyResult == PasswordVerificationResult.Failed) return false;

            // Hash mật khẩu mới
            user.PasswordHash = _passwordHasher.HashPassword(user, dto.NewPassword);
            user.UpdatedAt = DateTime.UtcNow;

            _userRepository.UpdateUser(user);
            await _userRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAccountAsync(int userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null) return false;

            // Soft Delete: Đổi status thành Deleted
            _userRepository.DeleteUser(user);
            await _userRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ToggleNewsletterAsync(int userId, bool subscribe)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null) return false;

            user.IsNewsletterSubscribed = subscribe;
            user.UpdatedAt = DateTime.UtcNow;

            _userRepository.UpdateUser(user);
            await _userRepository.SaveChangesAsync();
            return true;
        }
    }
}
