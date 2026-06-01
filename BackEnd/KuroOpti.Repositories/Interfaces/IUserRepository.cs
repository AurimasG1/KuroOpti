using KuroOpti.Entities;

namespace KuroOpti.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<int> CreateAsync(User user);
        Task<User?> GetByIdAsync(int id);
        Task<User?> GetByEmailAsync(string email);
        Task<List<User>> GetAllAsync(int page, int itemsPerPage);
        Task UpdateEmailAsync(int userId, string newEmail);
        Task UpdatePasswordHashAsync(int userId, string newPasswordHash);
        Task DeleteAsync(int id);
        Task<int> CountAsync();
        Task<List<User>> GetAllAsync();
        Task SaveChangesAsync();
    }
}
