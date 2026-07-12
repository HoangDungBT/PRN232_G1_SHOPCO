
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace SHOP.CO.Application.Utilities
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string toEmail, string subjet, string body);
    }


    public class EmailSender : IEmailSender
    {
        private readonly EmailSettings _emailSettings;
        public EmailSender(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
            // 🟢 Debug để xem app đang đọc cái gì
            Console.WriteLine("DEBUG: Email: " + _emailSettings.Email);
            // Chỉ in 3 ký tự đầu của mật khẩu để biết có nhận được mật khẩu không
            string passMask = string.IsNullOrEmpty(_emailSettings.Password) ? "NULL" : _emailSettings.Password.Substring(0, Math.Min(3, _emailSettings.Password.Length)) + "...";
            Console.WriteLine("DEBUG: Password start: " + passMask);
        }

        public async Task SendEmailAsync(string toEmail, string subjet, string body)
        {
            var email = new MimeMessage();
            Console.WriteLine("DEBUG: Host value: " + (_emailSettings.Host ?? "NULL"));
            email.From.Add(new MailboxAddress("SHOP.CO System", _emailSettings.Email));
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = subjet;

            var builder = new BodyBuilder { HtmlBody = body };
            email.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();

            // connect via TLS
            await smtp.ConnectAsync(_emailSettings.Host, _emailSettings.Port, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_emailSettings.Email, _emailSettings.Password);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);

        }
    }
}
