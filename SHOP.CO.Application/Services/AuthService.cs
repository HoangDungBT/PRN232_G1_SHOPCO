using Microsoft.AspNetCore.Http;
using SHOP.CO.Application.Common;
using System.Security.Claims;
namespace SHOP.CO.Application.Services
{
    public interface IAuthService
    {
        Task<ResultModel<string>> RegisterAsync(RegisterDto dto);
        Task<ResultModel<AuthResponseDto>> LoginAsync(LoginDto dto);

        Task<ResultModel<AuthResponseDto>> RefreshTokenAsync(RefreshTokenDto dto);
        Task<ResultModel<string>> LogoutAsync(int userId);

    }

    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepo;
        private readonly ITokenGenerator _tokenGenerator;
        private readonly IPasswordHasher _passwordHasher;

        protected readonly IHttpContextAccessor _http;

        public AuthService(
             IUserRepository userRepo,
             IPasswordHasher passwordHasher,
             ITokenGenerator tokenService,
             IHttpContextAccessor http)
        {
            _userRepo = userRepo;
            _passwordHasher = passwordHasher;
            _tokenGenerator = tokenService;
            _http = http;
        }



        protected int CurrentUserId()
        {
            return int.Parse(_http.HttpContext.User
                .FindFirst(ClaimTypes.NameIdentifier)!
                .Value);
        }

        protected string CurrentUserRole()
        {
            return _http.HttpContext.User
                .FindFirst("Role")!.Value;
        }

        public async Task<ResultModel<string>> RegisterAsync(RegisterDto dto)
        {
            try
            {
                if (await _userRepo.EmailExistsAsync(dto.Email))
                {
                    return ResultModel<string>.Error("Email này đã được sử dụng!", 400);
                }

                var user = new User
                {
                    FullName = dto.FullName,
                    Email = dto.Email,
                    PasswordHash = _passwordHasher.HashPassword(dto.Password),
                    Role = "Customer",
                    Status = "Active",
                    CreatedAt = DateTime.UtcNow
                };

                await _userRepo.AddUserAsync(user);
                return ResultModel<string>.Success(null, "Đăng ký tài khoản thành công!", 201);
            }
            catch (Exception ex)
            {

                return ResultModel<string>.Exception(ex);
            }
        }

        public async Task<ResultModel<AuthResponseDto>> LoginAsync(LoginDto dto)
        {
            try
            {

                var user = await _userRepo.GetUserByEmailAsync(dto.Email);

                if (user == null) return ResultModel<AuthResponseDto>.Error("Sai tài khoản hoặc mật khẩu!", 400);

                if (user.Status == "Locked" || user.Status == "Deleted")
                    return ResultModel<AuthResponseDto>.Error("Tài khoản của bạn đã bị khóa hoặc vô hiệu hóa!", 403);

                if (!_passwordHasher.VerifyPassword(dto.Password, user.PasswordHash))
                    return ResultModel<AuthResponseDto>.Error("Sai tài khoản hoặc mật khẩu!", 400);

                var accessToken = _tokenGenerator.GenerateAccessToken(user);
                var refreshToken = _tokenGenerator.GenerateRefreshToken();

                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(7);
                user.LastLoginAt = DateTime.UtcNow;

                await _userRepo.UpdateUserAsync(user);

                var data = new AuthResponseDto
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken
                };

                return ResultModel<AuthResponseDto>.Success(data ,"Đăng nhập thành công!", 200);
            }
            catch (Exception ex)
            {

                return ResultModel<AuthResponseDto>.Exception(ex);
            }
        }

        public async Task<ResultModel<AuthResponseDto>> RefreshTokenAsync(RefreshTokenDto dto)
        {
            try
            {
                // 1. Tìm User có chứa RefreshToken này
                var user = await _userRepo.GetUserByRefreshTokenAsync(dto.RefreshToken);

                // 2. Kiểm tra Token có tồn tại và còn hạn không
                if (user == null || user.RefreshTokenExpiresAt == null || user.RefreshTokenExpiresAt <= DateTime.UtcNow)
                {
                    return ResultModel<AuthResponseDto>.Error("Refresh Token không hợp lệ hoặc đã hết hạn. Vui lòng đăng nhập lại!", 401);
                }

                // 3. Sinh cặp Token mới
                var newAccessToken = _tokenGenerator.GenerateAccessToken(user);
                var newRefreshToken = _tokenGenerator.GenerateRefreshToken();

                // 4. Cập nhật lại vào DB
                user.RefreshToken = newRefreshToken;
                user.RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(7);
                await _userRepo.UpdateUserAsync(user);

                // 5. Trả về
                var data = new AuthResponseDto { AccessToken = newAccessToken, RefreshToken = newRefreshToken };
                return ResultModel<AuthResponseDto>.Success(data, "Làm mới token thành công!");
            }
            catch (Exception ex)
            {
                return ResultModel<AuthResponseDto>.Exception(ex);
            }
        }

        public async Task<ResultModel<string>> LogoutAsync(int userId)
        {
            try
            {
                var  user = await _userRepo.GetUserByIdAsync(userId);

                if (user != null)
                {
                    // xóa hoàn toàn Token 
                    user.RefreshToken = null;
                    user.RefreshTokenExpiresAt = null;
                    await _userRepo.UpdateUserAsync(user);
                }
                    return ResultModel<string>.Success(null, "Đăng xuất thành công!"); 
            }
            catch (Exception ex)
            {
                return ResultModel<string>.Exception(ex);
            }
         }
    }
}
