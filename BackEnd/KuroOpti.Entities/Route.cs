namespace KuroOpti.Entities
{
    public class Route
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = default!;

        public string? Name { get; set; }

        public double StartLat { get; set; }
        public double StartLng { get; set; }
        public double EndLat { get; set; }
        public double EndLng { get; set; }

        public string Polyline { get; set; } = default!;

        public ICollection<SearchLog> SearchLogs { get; set; } = new List<SearchLog>();
        public ICollection<UserRouteStation> SelectedStations { get; set; } =
            new List<UserRouteStation>();
    }
}
