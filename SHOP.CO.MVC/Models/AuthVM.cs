using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SHOP.CO.MVC.Models
{
    public class LoginVM
    {
        [Required(ErrorMessage = "Vui lòng nhập Email!")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập Mật khẩu!")]
        public string Password { get; set; } = string.Empty;
    }

    public class RegisterVM
    {
        [Required] public string FullName { get; set; } = string.Empty;
        [Required] public string Email { get; set; } = string.Empty;
        [Required] public string Password { get; set; } = string.Empty;
    }

    // Class bọc kết quả giống hệt ResultModel bên Application
    public class ApiResponse<T>
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        public bool IsSuccess
        {
            get => Success;
            set => Success = value;
        }

        public int Code { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; } = string.Empty;

        [JsonPropertyName("data")]
        public T? Data { get; set; }
    }

    public class TokenResponse
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }
}
