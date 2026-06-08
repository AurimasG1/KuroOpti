namespace KuroOpti.Common.Config
{
    public class EmailSettings
    {
        public string Host { get; set; } = default!;
        public int Port { get; set; }
        public bool EnableSsl { get; set; }
        public string User { get; set; } = default!;
        public string Password { get; set; } = default!;
    }
}
