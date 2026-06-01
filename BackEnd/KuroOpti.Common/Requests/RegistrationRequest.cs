namespace KuroOpti.Common.Requests
{
    public class RegistrationRequest
    {
        public required string Email { get; set; } = default!; 
        public required string Password { get; set; } = default!;  
        public string? AdminCode { get; set; }
    }
}
