using KuroOpti.Entities;

namespace KuroOpti.Repositories.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task SaveAsync(RefreshToken token);
        Task<RefreshToken?> GetAsync(string token);
        Task RevokeAsync(string token);
    }
}
