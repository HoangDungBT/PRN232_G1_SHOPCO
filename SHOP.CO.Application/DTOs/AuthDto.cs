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

        [MinLength(8, ErrorMessage = "Mật khẩu cần ít nhất 8 kí tự")]
        public string Password { get; set; } = string.Empty;
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
}
