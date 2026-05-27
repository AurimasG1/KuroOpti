using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using KuroOpti.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace KuroOpti.Services.Implementations
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration configuration;
        private readonly JwtSecurityTokenHandler tokenHandler = new();

        private readonly string issuer;
        private readonly string audience;
        private readonly string secretKey;
        private readonly int accessTokenLifetimeMinutes;
        private readonly int refreshTokenLifetimeDays;

        public TokenService(IConfiguration configuration)
        {
            this.configuration = configuration;

            issuer = configuration["Jwt:Issuer"]!;
            audience = configuration["Jwt:Audience"]!;
            secretKey = configuration["Jwt:SecretKey"]!;
            accessTokenLifetimeMinutes = int.Parse(
                configuration["Jwt:AccessTokenLifetimeMinutes"]!
            );
            refreshTokenLifetimeDays = int.Parse(configuration["Jwt:RefreshTokenLifetimeDays"]!);
        }

        public string GenerateAccessToken(IEnumerable<Claim> claims)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expires = DateTime.UtcNow.AddMinutes(accessTokenLifetimeMinutes);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: expires,
                signingCredentials: creds
            );

            return tokenHandler.WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomBytes = RandomNumberGenerator.GetBytes(64);
            return Convert.ToBase64String(randomBytes);
        }

        public DateTime GetAccessTokenExpiry()
        {
            return DateTime.UtcNow.AddMinutes(accessTokenLifetimeMinutes);
        }

        public DateTime GetRefreshTokenExpiry()
        {
            return DateTime.UtcNow.AddDays(refreshTokenLifetimeDays);
        }
    }
}
