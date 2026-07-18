using System;

namespace SHOP.CO.MVC.Models
{
    public class UserAddressViewModel
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
        public string AddressType { get; set; } = "Home"; // Dynamic label: Home, Office, Other
    }
}
