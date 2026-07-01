namespace SHOP.CO.MVC.Models
{
    public class NotificationViewModel
    {
        public int LogId { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; }
        public string CreatedAt { get; set; }
    }
}