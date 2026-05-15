namespace KuroOpti.Entities
{
    public class Route
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = default!;

        public string? Name { get; set; }

        public decimal StartLat { get; set; }
        public decimal StartLng { get; set; }
        public decimal EndLat { get; set; }
        public decimal EndLng { get; set; }

        public string Polyline { get; set; } = default!;

        public ICollection<SearchLog> SearchLogs { get; set; } = new List<SearchLog>();
    }
}
