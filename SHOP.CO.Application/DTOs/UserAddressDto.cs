using System;

namespace SHOP.CO.Application.DTOs
{
    public class UserAddressDto
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
    }
}
