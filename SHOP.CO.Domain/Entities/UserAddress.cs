using System;

namespace SHOP.CO.Domain.Entities
{
    public class UserAddress
    {
        public int AddressId { get; set; }
        public int UserId { get; set; } 
        public string ReceiverName { get; set; } = null!; 
        public string ReceiverPhone { get; set; } = null!; 
        public string Province { get; set; } = null!; 
        public string District { get; set; } = null!; 
        public string Ward { get; set; } = null!; 
        public string StreetAddress { get; set; } = null!; 
        public string? PostalCode { get; set; }
        public bool IsDefault { get; set; } 
        public DateTime CreatedAt { get; set; } 
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual User User { get; set; } = null!;
    }
}
