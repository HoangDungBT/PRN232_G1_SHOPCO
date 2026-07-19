using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SHOP.CO.Application.DTOs
{

    #region Register reg
    public class RegisterDto
    {
        [Required(ErrorMessage = "Vui lòng nhập họ tên!")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập Email!")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ!")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu!")]
        [MinLength(8, ErrorMessage = "Mật khẩu cần ít nhất 8 kí tự")]
        public string Password { get; set; } = string.Empty;
    }
    #endregion

    public class LoginDto
    {

        [Required(ErrorMessage = "Vui lòng nhập Email!")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ!")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu!")]
        [MinLength(8, ErrorMessage = "Mật khẩu cần ít nhất 8 kí tự")]
        public string Password { get; set; } = string.Empty;
    }
    public class VerifyEmailDto
    {
        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mã OTP là bắt buộc")]
        public string OtpCode { get; set; } = string.Empty;
    }

    // 🟢 DTO DÙNG ĐỂ YÊU CẦU GỬI LẠI MÃ
    public class ResendOtpDto
    {
        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
    public class AuthResponseDto
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }

    public class RefreshTokenDto
    {
        public string RefreshToken { get; set; } = string.Empty;
    }
    // DTO Dùng để yêu cầu cấp lại mật khẩu
    public class ForgotPasswordDto
    {
        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }

    // DTO Dùng để đặt lại mật khẩu mới
    public class ResetPasswordDto
    {
        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mã OTP là bắt buộc")]
        public string OtpCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu mới là bắt buộc")]
        [MinLength(6, ErrorMessage = "Mật khẩu phải từ 6 ký tự")]
        public string NewPassword { get; set; } = string.Empty;
    }
}
