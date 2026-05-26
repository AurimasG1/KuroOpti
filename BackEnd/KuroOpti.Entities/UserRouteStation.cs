namespace KuroOpti.Entities
{
    public class UserRouteStation
    {
        public int Id { get; set; }

        public int RouteId { get; set; }
        public Route Route { get; set; } = default!;

        public int FuelStationId { get; set; }
        public FuelStation FuelStation { get; set; } = default!;

        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    }
}
