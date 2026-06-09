namespace KuroOpti.Common.DTO
{
    public class RoutePlanningHistoryWithStationsDto
    {
        public int Id { get; set; }
        public int RouteId { get; set; }
        public DateTime PlannedAt { get; set; }

        public List<FuelStationDto> Stations { get; set; } = new();
        public string StartAddress { get; set; } = default!;
        public string EndAddress { get; set; } = default!;
        public double StartLat { get; set; }
        public double StartLng { get; set; }
        public double EndLat { get; set; }
        public double EndLng { get; set; }
    }
}
