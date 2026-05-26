using KuroOpti.Entities;

namespace KuroOpti.Services.Interfaces
{
    public interface IRefreshTokenService
    {
        Task<string> CreateRefreshTokenAsync(int userId);
        Task<User?> ValidateRefreshTokenAsync(string token);
    }
}
