namespace KuroOpti.Common.DTO
{
    public class RoutePlanningHistoryDto
    {
        public int Id { get; set; }
        public int RouteId { get; set; }
        public DateTime PlannedAt { get; set; }
        public List<int> SelectedStations { get; set; } = new();
    }
}
