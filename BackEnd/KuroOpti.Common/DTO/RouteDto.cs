namespace KuroOpti.Common.DTO
{
    public class RouteDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }

        public double StartLat { get; set; }
        public double StartLng { get; set; }
        public double EndLat { get; set; }
        public double EndLng { get; set; }

        public string Polyline { get; set; } = default!;
    }
}
