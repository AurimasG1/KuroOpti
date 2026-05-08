namespace KuroOpti.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; } = default!;
        public string PasswordHash { get; set; } = default!;
        public string Role { get; set; } = "user";

        public ICollection<Route> Routes { get; set; } = new List<Route>();
        public ICollection<SearchLog> SearchLogs { get; set; } = new List<SearchLog>();
    }
}
