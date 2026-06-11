namespace KuroOpti.Common.Requests
{
    public class CreateRouteRequest
    {
        public double StartLat { get; set; }
        public double StartLng { get; set; }
        public double EndLat { get; set; }
        public double EndLng { get; set; }
        public string? Name { get; set; }
        public string Polyline { get; set; } = default!;
    }
}
