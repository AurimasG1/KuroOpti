using KuroOpti.Entities;
using KuroOpti.Repositories.Interfaces;
using KuroOpti.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace KuroOpti.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUserRepository userRepository;
        private readonly PasswordHasher<User> passwordHasher;

        public UserService(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
            this.passwordHasher = new PasswordHasher<User>();
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await userRepository.GetByEmailAsync(email);
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await userRepository.GetByIdAsync(id);
        }

        public async Task<User?> RegisterAsync(string email, string password)
        {
            var existing = await userRepository.GetByEmailAsync(email);
            if (existing != null)
                return null;

            var user = new User { Email = email.ToLowerInvariant().Trim(), Role = "user" };
            user.PasswordHash = passwordHasher.HashPassword(user, password);

            await userRepository.CreateAsync(user);
            return user;
        }

        public async Task<User?> ValidateUserAsync(string email, string password)
        {
            var user = await userRepository.GetByEmailAsync(email.Trim().ToLowerInvariant());
            if (user == null)
                return null;

            var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);

            if (result == PasswordVerificationResult.Failed)
                return null;

            return user;
        }
    }
}
