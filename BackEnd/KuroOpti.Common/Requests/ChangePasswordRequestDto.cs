namespace KuroOpti.Common.Requests
{
    public class ChangePasswordRequestDto
    {
        public string OldPassword { get; set; } = default!;
        public string NewPassword { get; set; } = default!;
    }
}
