using System.ComponentModel.DataAnnotations;

namespace SHOP.CO.Application.DTOs
{
    public class CreateReviewRequest
    {
        [Range(1, 5, ErrorMessage = "Đánh giá phải từ 1 đến 5 sao.")]
        public int Rating { get; set; }

        [Required(ErrorMessage = "Bình luận không được để trống.")]
        [StringLength(1000, ErrorMessage = "Bình luận không vượt quá 1000 ký tự.")]
        public string Comment { get; set; } = null!;
    }
}
