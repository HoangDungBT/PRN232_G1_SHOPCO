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

        Task<ResultModel<string>> VerifyEmailAsync(VerifyEmailDto dto);
        Task<ResultModel<string>> ResendVerificationEmailAsync(ResendOtpDto dto);
        Task<ResultModel<string>> ForgotPasswordAsync(ForgotPasswordDto dto);
        Task<ResultModel<string>> ResetPasswordAsync(ResetPasswordDto dto);

    }

    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepo;
        private readonly ITokenGenerator _tokenGenerator;
        private readonly IPasswordHasher _passwordHasher;
        protected readonly IHttpContextAccessor _http;
        private readonly IEmailSender _emailSender;

        public AuthService(
             IUserRepository userRepo,
             IPasswordHasher passwordHasher,
             ITokenGenerator tokenService,
             IHttpContextAccessor http,
            IEmailSender emailSender
            )
        {
            _userRepo = userRepo;
            _passwordHasher = passwordHasher;
            _tokenGenerator = tokenService;
            _http = http;
            _emailSender = emailSender;
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
                // 🟢 1. SINH MÃ OTP 6 SỐ
              string otpCode = GenerateRandomOtp(6);

                var user = new User
                {
                    FullName = dto.FullName,
                    Email = dto.Email,
                    PasswordHash = _passwordHasher.HashPassword(dto.Password),
                    VerificationToken = otpCode,
                    VerificationExpiresAt = DateTime.UtcNow.AddMinutes(15),
                    Role = "Customer",
                    Status = "Unverified",
                    CreatedAt = DateTime.UtcNow
                };
                string emailBody = $@"
                    <div style='font-family: Arial, sans-serif; padding: 20px;'>
                        <h2>Chào mừng đến với SHOP.CO!</h2>
                        <p>Xin chào <strong>{user.FullName}</strong>,</p>
                        <p>Cảm ơn bạn đã đăng ký tài khoản. Mã xác minh (OTP) của bạn là:</p>
                        <h1 style='color: #0d6efd; letter-spacing: 5px;'>{otpCode}</h1>
                        <p>Mã này sẽ hết hạn sau <strong>15 phút</strong>.</p>
                        <p>Vui lòng không chia sẻ mã này cho bất kỳ ai.</p>
                    </div>";
                await _userRepo.AddAsync(user);

                await _emailSender.SendEmailAsync(user.Email, "Xác minh tài khoản SHOP.CO", emailBody);

                return ResultModel<string>.Success(null, "Đăng ký thành công! Vui lòng kiểm tra Email để lấy mã xác minh.", 201);
            }
            catch (Exception ex)
            {

                var innerEx = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return ResultModel<string>.Error(innerEx, 500);
                //return ResultModel<string>.Exception(ex);
            }
        }

        public async Task<ResultModel<AuthResponseDto>> LoginAsync(LoginDto dto)
        {
            try
            {

                var user = await _userRepo.GetUserByEmailAsync(dto.Email);

                if (user == null) return ResultModel<AuthResponseDto>.Error("Sai tài khoản hoặc mật khẩu!", 400);

                // 🟢 CHẶN ĐĂNG NHẬP NẾU CHƯA XÁC MINH EMAIL
                if (user.Status == "Unverified")
                    return ResultModel<AuthResponseDto>.Error("Tài khoản chưa được xác minh. Vui lòng kiểm tra Email!", 403);

                if (user.Status == "Locked" || user.Status == "Deleted")
                    return ResultModel<AuthResponseDto>.Error("Tài khoản của bạn đã bị khóa hoặc vô hiệu hóa!", 403);

                if (!_passwordHasher.VerifyPassword(dto.Password, user.PasswordHash))
                    return ResultModel<AuthResponseDto>.Error("Sai tài khoản hoặc mật khẩu!", 400);

                var accessToken = _tokenGenerator.GenerateAccessToken(user);
                var refreshToken = _tokenGenerator.GenerateRefreshToken();

                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(7);
                user.LastLoginAt = DateTime.UtcNow;

                await _userRepo.UpdateAsync(user);

                var data = new AuthResponseDto
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken
                };

                return ResultModel<AuthResponseDto>.Success(data, "Đăng nhập thành công!", 200);
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
                await _userRepo.UpdateAsync(user);

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
                var user = await _userRepo.GetByIdAsync(userId);

                if (user != null)
                {
                    // xóa hoàn toàn Token 
                    user.RefreshToken = null;
                    user.RefreshTokenExpiresAt = null;
                    await _userRepo.UpdateAsync(user);
                }
                return ResultModel<string>.Success(null, "Đăng xuất thành công!");
            }
            catch (Exception ex)
            {
                return ResultModel<string>.Exception(ex);
            }
        }
        public async Task<ResultModel<string>> VerifyEmailAsync(VerifyEmailDto dto)
        {
            try
            {
                var user = await _userRepo.GetUserByEmailAsync(dto.Email);
                if (user == null) return ResultModel<string>.Error("Không tìm thấy tài khoản!", 404);

                if (user.Status != "Unverified") return ResultModel<string>.Error("Tài khoản này đã được xác minh trước đó!", 400);

                if (user.VerificationToken != dto.OtpCode) return ResultModel<string>.Error("Mã xác minh không chính xác!", 400);

                if (user.VerificationExpiresAt < DateTime.UtcNow) return ResultModel<string>.Error("Mã xác minh đã hết hạn. Vui lòng yêu cầu gửi lại mã mới!", 400);

                // Xác minh thành công -> Đổi Status và xóa Token
                user.Status = "Active";
                user.VerificationToken = null;
                user.VerificationExpiresAt = null;

                await _userRepo.UpdateAsync(user);

                return ResultModel<string>.Success(null, "Xác minh Email thành công! Bạn có thể đăng nhập ngay bây giờ.", 200);
            }
            catch (Exception ex) { return ResultModel<string>.Exception(ex); }
        }

        // 🟢 HÀM YÊU CẦU GỬI LẠI MÃ OTP
        public async Task<ResultModel<string>> ResendVerificationEmailAsync(ResendOtpDto dto)
        {
            try
            {
                var user = await _userRepo.GetUserByEmailAsync(dto.Email);
                if (user == null) return ResultModel<string>.Error("Không tìm thấy tài khoản!", 404);

                if (user.Status != "Unverified") return ResultModel<string>.Error("Tài khoản này đã được xác minh trước đó!", 400);

                // Sinh mã mới
                string newOtp = new Random().Next(100000, 999999).ToString();
                user.VerificationToken = newOtp;
                user.VerificationExpiresAt = DateTime.UtcNow.AddMinutes(15);
                await _userRepo.UpdateAsync(user);

                // Gửi lại Email
                string emailBody = $@"
                    <div style='font-family: Arial, sans-serif; padding: 20px;'>
                        <h2>Gửi lại mã xác minh SHOP.CO!</h2>
                        <p>Xin chào <strong>{user.FullName}</strong>,</p>
                        <p>Bạn vừa yêu cầu gửi lại mã xác minh. Mã OTP mới của bạn là:</p>
                        <h1 style='color: #dc3545; letter-spacing: 5px;'>{newOtp}</h1>
                        <p>Mã này sẽ hết hạn sau <strong>15 phút</strong>.</p>
                    </div>";

                await _emailSender.SendEmailAsync(user.Email, "Mã xác minh mới từ SHOP.CO", emailBody);

                return ResultModel<string>.Success(null, "Đã gửi lại mã xác minh vào Email của bạn.", 200);
            }
            catch (Exception ex) { return ResultModel<string>.Exception(ex); }
        }
        
        // 🟢 HÀM 1: GỬI OTP QUÊN MẬT KHẨU
        public async Task<ResultModel<string>> ForgotPasswordAsync(ForgotPasswordDto dto)
        {
            try
            {
                var user = await _userRepo.GetUserByEmailAsync(dto.Email);
                if (user == null) return ResultModel<string>.Error("Không tìm thấy tài khoản với email này!", 404);

                // Sinh mã OTP 6 số
                string otpCode = new Random().Next(100000, 999999).ToString();

                // Lưu vào cột ResetPassword
                user.ResetPasswordToken = otpCode;
                user.ResetPasswordExpiresAt = DateTime.UtcNow.AddMinutes(15);
                await _userRepo.UpdateAsync(user);

                // Gửi Email
                string emailBody = $@"
                    <div style='font-family: Arial, sans-serif; padding: 20px;'>
                        <h2>Yêu cầu đặt lại mật khẩu!</h2>
                        <p>Xin chào <strong>{user.FullName}</strong>,</p>
                        <p>Chúng tôi nhận được yêu cầu đặt lại mật khẩu cho tài khoản của bạn. Mã OTP của bạn là:</p>
                        <h1 style='color: #dc3545; letter-spacing: 5px;'>{otpCode}</h1>
                        <p>Mã này sẽ hết hạn sau <strong>15 phút</strong>. Nếu bạn không yêu cầu, vui lòng bỏ qua email này.</p>
                    </div>";

                await _emailSender.SendEmailAsync(user.Email, "Khôi phục mật khẩu SHOP.CO", emailBody);

                return ResultModel<string>.Success(null, "Mã OTP khôi phục đã được gửi vào Email của bạn.", 200);
            }
            catch (Exception ex) { return ResultModel<string>.Exception(ex); }
        }

        // 🟢 HÀM 2: ĐẶT LẠI MẬT KHẨU MỚI BẰNG OTP
        public async Task<ResultModel<string>> ResetPasswordAsync(ResetPasswordDto dto)
        {
            try
            {
                var user = await _userRepo.GetUserByEmailAsync(dto.Email);
                if (user == null) return ResultModel<string>.Error("Không tìm thấy tài khoản!", 404);

                if (user.ResetPasswordToken != dto.OtpCode) return ResultModel<string>.Error("Mã OTP không chính xác!", 400);
                if (user.ResetPasswordExpiresAt < DateTime.UtcNow) return ResultModel<string>.Error("Mã OTP đã hết hạn!", 400);

                // Cập nhật mật khẩu mới và xóa OTP
                user.PasswordHash = _passwordHasher.HashPassword(dto.NewPassword);
                user.ResetPasswordToken = null;
                user.ResetPasswordExpiresAt = null;

                // Nếu tài khoản chưa verify, verify luôn cho họ
                if (user.Status == "Unverified") user.Status = "Active";

                await _userRepo.UpdateAsync(user);

                return ResultModel<string>.Success(null, "Đặt lại mật khẩu thành công! Bạn có thể đăng nhập ngay.", 200);
            }
            catch (Exception ex) { return ResultModel<string>.Exception(ex); }
        }
        private string GenerateRandomOtp(int length)
        {
            using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
            byte[] bytes = new byte[length];
            rng.GetBytes(bytes);
            return string.Concat(bytes.Select(b => (b % 10).ToString()));
        }
    }
}
