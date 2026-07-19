using SHOP.CO.Domain.Entities;
using SHOP.CO.Application.Utilities;
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
        Task<UserAddressDto?> AddUserAddressAsync(int userId, UserAddressDto dto);
        Task<bool> UpdateUserAddressAsync(int userId, UserAddressDto dto);
        Task<bool> DeleteUserAddressAsync(int userId, int addressId);
        Task<bool> SetDefaultAddressAsync(int userId, int addressId);
    }

    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;

        public UserService(IUserRepository userRepository, IPasswordHasher passwordHasher)
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
            var verifyResult = _passwordHasher.VerifyPassword(dto.CurrentPassword, user.PasswordHash);
            if (!verifyResult) return false;

            // Hash mật khẩu mới
            user.PasswordHash = _passwordHasher.HashPassword(dto.NewPassword);
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

        public async Task<UserAddressDto?> AddUserAddressAsync(int userId, UserAddressDto dto)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null) return null;

            var newAddress = new UserAddress
            {
                UserId = userId,
                ReceiverName = dto.ReceiverName ?? user.FullName,
                ReceiverPhone = dto.ReceiverPhone ?? user.Phone ?? string.Empty,
                Province = dto.Province ?? string.Empty,
                District = dto.District ?? string.Empty,
                Ward = dto.Ward ?? string.Empty,
                StreetAddress = dto.StreetAddress ?? string.Empty,
                PostalCode = dto.PostalCode,
                IsDefault = dto.IsDefault || (user.UserAddresses == null || !user.UserAddresses.Any(a => a.DeletedAt == null)),
                CreatedAt = DateTime.UtcNow
            };

            if (newAddress.IsDefault && user.UserAddresses != null)
            {
                foreach (var a in user.UserAddresses) a.IsDefault = false;
            }

            if (user.UserAddresses == null) user.UserAddresses = new List<UserAddress>();
            user.UserAddresses.Add(newAddress);
            
            _userRepository.UpdateUser(user);
            await _userRepository.SaveChangesAsync();

            dto.AddressId = newAddress.AddressId;
            return dto;
        }

        public async Task<bool> UpdateUserAddressAsync(int userId, UserAddressDto dto)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null || user.UserAddresses == null) return false;

            var address = user.UserAddresses.FirstOrDefault(a => a.AddressId == dto.AddressId && a.DeletedAt == null);
            if (address == null) return false;

            address.ReceiverName = dto.ReceiverName ?? address.ReceiverName;
            address.ReceiverPhone = dto.ReceiverPhone ?? address.ReceiverPhone;
            address.Province = dto.Province ?? address.Province;
            address.District = dto.District ?? address.District;
            address.Ward = dto.Ward ?? address.Ward;
            address.StreetAddress = dto.StreetAddress ?? address.StreetAddress;
            address.PostalCode = dto.PostalCode ?? address.PostalCode;
            address.UpdatedAt = DateTime.UtcNow;

            if (dto.IsDefault && !address.IsDefault)
            {
                foreach (var a in user.UserAddresses) a.IsDefault = false;
                address.IsDefault = true;
            }

            _userRepository.UpdateUser(user);
            await _userRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteUserAddressAsync(int userId, int addressId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null || user.UserAddresses == null) return false;

            var address = user.UserAddresses.FirstOrDefault(a => a.AddressId == addressId && a.DeletedAt == null);
            if (address == null) return false;

            address.DeletedAt = DateTime.UtcNow;

            if (address.IsDefault)
            {
                address.IsDefault = false;
                var newDefault = user.UserAddresses.FirstOrDefault(a => a.DeletedAt == null);
                if (newDefault != null) newDefault.IsDefault = true;
            }

            _userRepository.UpdateUser(user);
            await _userRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SetDefaultAddressAsync(int userId, int addressId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null || user.UserAddresses == null) return false;

            var address = user.UserAddresses.FirstOrDefault(a => a.AddressId == addressId && a.DeletedAt == null);
            if (address == null) return false;

            foreach (var a in user.UserAddresses) a.IsDefault = false;
            address.IsDefault = true;

            _userRepository.UpdateUser(user);
            await _userRepository.SaveChangesAsync();
            return true;
        }
    }
}
