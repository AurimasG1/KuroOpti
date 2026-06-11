namespace KuroOpti.Common.DTO
{
    public class RoutePlanningHistoryDto
    {
        public int Id { get; set; }
        public int RouteId { get; set; }
        public DateTime CreatedAt { get; set; }

        public string StartAddress { get; set; } = default!;
        public string EndAddress { get; set; } = default!;
        public double StartLat { get; set; }
        public double StartLng { get; set; }
        public double EndLat { get; set; }
        public double EndLng { get; set; }
        public string FuelType { get; set; } = default!;
        public double DistanceKm { get; set; }
        public string Polyline { get; set; } = default!;
        public double FuelEstimate { get; set; }
        public List<FuelStationDto> Stations { get; set; } = new();
    }
}
