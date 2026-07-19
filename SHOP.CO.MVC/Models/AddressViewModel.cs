namespace SHOP.CO.MVC.Models
{
    public class AddressViewModel
    {
        public int AddressId { get; set; }
        public string ReceiverName { get; set; }
        public string ReceiverPhone { get; set; }
        public string Province { get; set; }
        public string District { get; set; }
        public string Ward { get; set; }
        public string StreetAddress { get; set; }
        public bool IsDefault { get; set; }
    }
}