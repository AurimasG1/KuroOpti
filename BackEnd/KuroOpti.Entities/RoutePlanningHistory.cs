using System.ComponentModel.DataAnnotations.Schema;

namespace KuroOpti.Entities
{
    public class RoutePlanningHistory
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = default!;

        public int RouteId { get; set; }
        public Route Route { get; set; } = default!;

        public DateTime PlannedAt { get; set; } = DateTime.UtcNow;

        public string SelectedStationsJson { get; set; } = default!;

        [NotMapped]
        public List<FuelStation> Stations { get; set; } = new();
    }
}
