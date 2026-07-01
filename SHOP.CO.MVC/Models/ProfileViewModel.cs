namespace SHOP.CO.MVC.Models
{
    public class ProfileViewModel
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Gender { get; set; }
        public string DateOfBirth { get; set; } // Dùng string để parse date dễ hơn
        public string PreferredSize { get; set; }
        public string PreferredStyle { get; set; }
        public bool IsNewsletterSubscribed { get; set; }
    }
}