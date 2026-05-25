namespace KuroOpti.Services.Interfaces
{
    using System.Security.Claims;

    namespace KuroOpti.Services.Interfaces
    {
        public interface ITokenService
        {
            string GenerateAccessToken(IEnumerable<Claim> claims);
            string GenerateRefreshToken();
            DateTime GetAccessTokenExpiry();
            DateTime GetRefreshTokenExpiry();
        }
    }
}
