using KuroOpti.Entities;
using KuroOpti.Repositories.Interfaces;
using KuroOpti.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace KuroOpti.Services.Implementations
{
    public class PasswordResetService : IPasswordResetService
    {
        private readonly IUserRepository userRepo;
        private readonly IPasswordResetRepository resetRepo;
        private readonly IEmailService emailService;

        public PasswordResetService(
            IUserRepository userRepo,
            IPasswordResetRepository resetRepo,
            IEmailService emailService
        )
        {
            this.userRepo = userRepo;
            this.resetRepo = resetRepo;
            this.emailService = emailService;
        }

        public async Task RequestPasswordResetAsync(string email, string baseResetUrl)
        {
            var user = await userRepo.GetByEmailAsync(email);
            if (user == null)
            {
                // Saugumo sumetimais nieko nesakome – visada grąžinam OK
                return;
            }

            var token = Guid.NewGuid().ToString("N");

            var reset = new PasswordResetToken
            {
                UserId = user.Id,
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddMinutes(15),
            };

            await resetRepo.AddAsync(reset);

            var link = $"{baseResetUrl}?token={token}";
            await emailService.SendPasswordResetEmailAsync(user.Email, link);
        }

        public async Task<bool> ResetPasswordAsync(string token, string newPassword)
        {
            var reset = await resetRepo.GetValidTokenAsync(token);
            if (reset == null)
                return false;

            var user = reset.User;
            if (user == null)
                return false;

            var hasher = new PasswordHasher<User>();
            user.PasswordHash = hasher.HashPassword(user, newPassword);

            await userRepo.SaveChangesAsync();
            await resetRepo.MarkAsUsedAsync(reset);

            return true;
        }
    }
}
