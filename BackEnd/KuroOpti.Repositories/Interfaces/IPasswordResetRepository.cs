using KuroOpti.Entities;

namespace KuroOpti.Repositories.Interfaces
{
    public interface IPasswordResetRepository
    {
        Task AddAsync(PasswordResetToken token);
        Task<PasswordResetToken?> GetValidTokenAsync(string token);
        Task MarkAsUsedAsync(PasswordResetToken token);
    }
}
