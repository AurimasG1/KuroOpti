using KuroOpti.Services.Interfaces;
using Microsoft.Extensions.Configuration;

namespace KuroOpti.Services.Implementations
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration config;

        public EmailService(IConfiguration config)
        {
            this.config = config;
        }

        public Task SendPasswordResetEmailAsync(string toEmail, string resetLink)
        {
            // Čia vėliau įdėsi realų siuntimą (SMTP / SendGrid / kt.)
            Console.WriteLine($"[DEV] Password reset link for {toEmail}: {resetLink}");
            return Task.CompletedTask;
        }
    }
}
