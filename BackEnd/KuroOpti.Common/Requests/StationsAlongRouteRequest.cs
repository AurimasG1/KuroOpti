namespace KuroOpti.Common.Requests
{
    public class StationsAlongRouteRequest
    {
        public string Polyline { get; set; } = default!;
        public string FuelType { get; set; } = default!;
        public decimal MaxDistanceKm { get; set; } = 5;
    }
}
