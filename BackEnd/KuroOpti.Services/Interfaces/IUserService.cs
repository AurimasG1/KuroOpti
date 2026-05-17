using KuroOpti.Entities;

namespace KuroOpti.Services.Interfaces
{
    public interface IUserService
    {
        Task<bool> RegisterAsync(string email, string password);
        Task<User?> ValidateUserAsync(string email, string password);
    }
}
