using System.Net;
using System.Net.Mail;
using KuroOpti.Common.Config;
using KuroOpti.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace KuroOpti.Services.Implementations
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings settings;

        public EmailService(IOptions<EmailSettings> settings)
        {
            this.settings = settings.Value;
        }

        public async Task SendPasswordResetEmailAsync(string toEmail, string resetLink)
        {
            using var client = new SmtpClient(settings.Host, settings.Port)
            {
                EnableSsl = settings.EnableSsl,
                Credentials = new NetworkCredential(settings.User, settings.Password),
            };

            var message = new MailMessage
            {
                From = new MailAddress(settings.User, "KuroOpti"),
                Subject = "Password Reset",
                Body = $"Reset your password using this link:\n{resetLink}",
                IsBodyHtml = false,
            };

            message.To.Add(toEmail);

            await client.SendMailAsync(message);
        }
    }
}
