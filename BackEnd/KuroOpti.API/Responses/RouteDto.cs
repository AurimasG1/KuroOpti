namespace KuroOpti.API.Responses
{
    public class RouteDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }

        public decimal StartLat { get; set; }
        public decimal StartLng { get; set; }
        public decimal EndLat { get; set; }
        public decimal EndLng { get; set; }

        public string Polyline { get; set; } = default!;
    }
}
