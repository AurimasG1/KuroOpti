using KuroOpti.Entities;

namespace KuroOpti.Services.Interfaces
{
    public interface IUserService
    {
        Task<User?> RegisterAsync(string email, string password);
        Task<User?> ValidateUserAsync(string email, string password);
        Task<User?> GetUserByEmailAsync(string email);
        Task<User?> GetUserByIdAsync(int id);
        Task<List<User>> GetAllUsersAsync(int page, int itemsPerPage);
        Task<bool> ChangeEmailAsync(int userId, string newEmail);
        Task<bool> ChangePasswordAsync(int userId, string oldPassword, string newPassword);
        Task DeleteUserAsync(int id);
    }
}
