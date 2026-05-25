using System;
using System.Collections.Generic;

namespace SHOP.CO.Domain.Entities
{
    public class User
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = null!; 
        public string Email { get; set; } = null!; 
        public string PasswordHash { get; set; } = null!; 
        public string? Phone { get; set; }
        public string? AvatarUrl { get; set; }
        public string Role { get; set; } = null!; 
        public string Status { get; set; } = null!; 
        public string? Gender { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public string? PreferredSize { get; set; }
        public string? PreferredStyle { get; set; }

        public string? ResetPasswordToken { get; set; }
        public DateTime? ResetPasswordExpiresAt { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiresAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public DateTime CreatedAt { get; set; } 
        public DateTime? UpdatedAt { get; set; }

        public virtual ICollection<UserAddress> UserAddresses { get; set; } = new List<UserAddress>();
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
        public virtual ICollection<InteractionLog> InteractionLogs { get; set; } = new List<InteractionLog>();
        public virtual ICollection<CustomerActivity> CustomerActivities { get; set; } = new List<CustomerActivity>();
        public virtual ICollection<CommerceRecord> CommerceRecords { get; set; } = new List<CommerceRecord>();
        public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    }
}
