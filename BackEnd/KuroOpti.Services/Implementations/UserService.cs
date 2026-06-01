using KuroOpti.Entities;
using KuroOpti.Repositories.Interfaces;
using KuroOpti.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace KuroOpti.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUserRepository userRepository;
        private readonly PasswordHasher<User> hasher = new();
        private readonly IConfiguration configuration;

        public UserService(IUserRepository userRepository, IConfiguration configuration)
        {
            this.userRepository = userRepository;
            this.configuration = configuration; 
        }

        public async Task<User?> RegisterAsync(string email, string password, string? adminCode) // mano adminCode
        {
            var existing = await userRepository.GetByEmailAsync(email);
            if (existing != null)
                return null;

            // idetas roles nustatymas pagal adminCode vietoje default user
            string role = "user";

            string configuredAdminCode = configuration["AdminSettings:AdminCode"];

            if (!string.IsNullOrWhiteSpace(adminCode) && adminCode == configuredAdminCode)
            {
                role = "admin";
            }

            var user = new User { Email = email.ToLowerInvariant().Trim(), Role = role };
            
            // var user = new User { Email = email.ToLowerInvariant().Trim(), Role = "user" };
            user.PasswordHash = hasher.HashPassword(user, password);

            await userRepository.CreateAsync(user);
            return user;
        }

        public async Task<User?> ValidateUserAsync(string email, string password)
        {
            var user = await userRepository.GetByEmailAsync(email.Trim().ToLowerInvariant());
            if (user == null)
                return null;

            var result = hasher.VerifyHashedPassword(user, user.PasswordHash, password);

            if (result == PasswordVerificationResult.Failed)
                return null;

            return user;
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await userRepository.GetByEmailAsync(email);
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await userRepository.GetByIdAsync(id);
        }

        public async Task<List<User>> GetAllUsersAsync(int page, int itemsPerPage)
        {
            return await userRepository.GetAllAsync(page, itemsPerPage);
        }

        public async Task<bool> ChangeEmailAsync(int userId, string newEmail)
        {
            var existing = await userRepository.GetByEmailAsync(newEmail);
            if (existing != null)
                return false; // email already used

            await userRepository.UpdateEmailAsync(userId, newEmail.ToLower().Trim());
            return true;
        }

        public async Task<bool> ChangePasswordAsync(
            int userId,
            string oldPassword,
            string newPassword
        )
        {
            var user = await userRepository.GetByIdAsync(userId);
            if (user == null)
                return false;

            var result = hasher.VerifyHashedPassword(user, user.PasswordHash, oldPassword);
            if (result == PasswordVerificationResult.Failed)
                return false;

            string newHash = hasher.HashPassword(user, newPassword);
            await userRepository.UpdatePasswordHashAsync(userId, newHash);

            return true;
        }

        public async Task DeleteUserAsync(int id)
        {
            await userRepository.DeleteAsync(id);
        }

        public async Task<int> GetTotalUsersAsync()
        {
            return await userRepository.CountAsync();
        }
    }
}
