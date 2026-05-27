using System.Security.Cryptography;
using KuroOpti.Entities;
using KuroOpti.Repositories.Interfaces;
using KuroOpti.Services.Interfaces;

namespace KuroOpti.Services.Implementations
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly IRefreshTokenRepository repo;
        private readonly IUserRepository userRepo;

        public RefreshTokenService(IRefreshTokenRepository repo, IUserRepository userRepo)
        {
            this.repo = repo;
            this.userRepo = userRepo;
        }

        public async Task<string> CreateRefreshTokenAsync(int userId)
        {
            var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

            var refresh = new RefreshToken
            {
                UserId = userId,
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                IsRevoked = false,
            };

            await repo.SaveAsync(refresh);
            return token;
        }

        public async Task<User?> ValidateRefreshTokenAsync(string token)
        {
            var rt = await repo.GetAsync(token);

            if (rt == null || rt.ExpiresAt < DateTime.UtcNow)
                return null;

            return await userRepo.GetByIdAsync(rt.UserId);
        }
    }
}
