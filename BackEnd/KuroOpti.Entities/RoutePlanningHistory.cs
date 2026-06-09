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
        public string StartAddress { get; set; } = default!;
        public string EndAddress { get; set; } = default!;
        public double StartLat { get; set; }
        public double StartLng { get; set; }
        public double EndLat { get; set; }
        public double EndLng { get; set; }
        public string FuelType { get; set; }
        public double DistanceKm { get; set; }

        public DateTime PlannedAt { get; set; } = DateTime.UtcNow;

        public string SelectedStationsJson { get; set; } = default!;

        [NotMapped]
        public List<FuelStation> Stations { get; set; } = new();
    }
}
