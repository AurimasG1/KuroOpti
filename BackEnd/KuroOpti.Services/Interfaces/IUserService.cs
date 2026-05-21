using KuroOpti.Entities;

namespace KuroOpti.Services.Interfaces
{
    public interface IUserService
    {
        Task<User?> RegisterAsync(string email, string password);
        Task<User?> ValidateUserAsync(string email, string password);
        Task<User?> GetUserByEmailAsync(string email);
        Task<User?> GetUserById(int id);
        // Task<List<User>> GetAllUsersAsync(int page, int itemsPerPage);
        // Task UpdateUser(User user);
        // Task DeleteUser(int id);
    }
}
