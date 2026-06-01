namespace KuroOpti.Services.Interfaces
{
    public interface IPasswordResetService
    {
        Task RequestPasswordResetAsync(string email, string baseResetUrl);
        Task<bool> ResetPasswordAsync(string token, string newPassword);
    }
}
