using KuroOpti.Common.DTO;

namespace KuroOpti.Common.Responses
{
    public class AuthResponse
    {
        public string AccessToken { get; set; } = default!;
        public string RefreshToken { get; set; } = default!;
        public UserDto User { get; set; } = default!;
    }
}
