namespace KuroOpti.Common.DTO
{
    public class RoutePlanningHistoryWithStationsDto
    {
        public int Id { get; set; }
        public int RouteId { get; set; }
        public DateTime PlannedAt { get; set; }

        public List<FuelStationDto> Stations { get; set; } = new();
    }
}
