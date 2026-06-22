
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
        }

        public async Task SendEmailAsync(string toEmail, string subjet, string body)
        {
            var email = new MimeMessage();
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
