using System.Security.Cryptography;
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

        // STEP 1: REQUEST PASSWORD RESET
        public async Task RequestPasswordResetAsync(string email, string baseResetUrl)
        {
            var user = await userRepo.GetByEmailAsync(email);

            // Saugumo sumetimais visada grąžinam OK
            if (user == null)
                return;

            // Saugus 64 baitų tokenas
            var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

            var reset = new PasswordResetToken
            {
                UserId = user.Id,
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddMinutes(15),
                Used = false,
            };

            await resetRepo.AddAsync(reset);

            var link = $"{baseResetUrl}?token={Uri.EscapeDataString(token)}";

            await emailService.SendPasswordResetEmailAsync(user.Email, link);
        }

        // STEP 2: RESET PASSWORD
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

            await userRepo.UpdatePasswordHashAsync(user.Id, user.PasswordHash);
            await resetRepo.MarkAsUsedAsync(reset);

            return true;
        }
    }
}
