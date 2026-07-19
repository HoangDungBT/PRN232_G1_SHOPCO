using System;

namespace SHOP.CO.Application.DTOs
{
    public class ReviewDto
    {
        public int ActivityId { get; set; }
        public int? UserId { get; set; }
        public string ReviewerName { get; set; } = null!;
        public int Rating { get; set; }
        public string Comment { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}
