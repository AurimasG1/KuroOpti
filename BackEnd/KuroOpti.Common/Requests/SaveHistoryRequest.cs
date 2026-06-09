namespace KuroOpti.Common.Requests
{
    public class SaveHistoryRequest
    {
        public int RouteId { get; set; }
        public string StartAddress { get; set; } = default!;
        public string EndAddress { get; set; } = default!;
        public double StartLat { get; set; }
        public double StartLng { get; set; }
        public double EndLat { get; set; }
        public double EndLng { get; set; }
        public string FuelType { get; set; } = default!;
        public double DistanceKm { get; set; }
        public List<int> SelectedStationIds { get; set; } = new();
    }
}
