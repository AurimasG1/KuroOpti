namespace KuroOpti.Common.DTO
{
    public class FuelStationDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public string Municipality { get; set; } = default!;
        public string Address { get; set; } = default!;
        public decimal DieselPrice { get; set; }
        public decimal PetrolPrice { get; set; }
        public decimal LpgPrice { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public double DistanceFromRouteKm { get; set; }
    }
}
