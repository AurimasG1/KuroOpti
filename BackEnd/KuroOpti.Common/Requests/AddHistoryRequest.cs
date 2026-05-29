namespace KuroOpti.Common.DTO
{
    public class AddHistoryRequest
    {
        public int RouteId { get; set; }
        public List<int> SelectedStationIds { get; set; } = new();
    }
}
