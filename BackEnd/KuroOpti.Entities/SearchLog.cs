namespace KuroOpti.Entities
{
    public class SearchLog
    {
        public int Id { get; set; }

        public int? UserId { get; set; }
        public User? User { get; set; }

        public int? RouteId { get; set; }
        public Route? Route { get; set; }

        public DateTime SearchTime { get; set; }
        public int ResultsCount { get; set; }
        public decimal? MinPriceFound { get; set; }
    }
}
