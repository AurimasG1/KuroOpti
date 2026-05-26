namespace KuroOpti.Services.Interfaces
{
    public interface IGeocodingService
    {
        Task<(decimal Latitude, decimal Longitude)> GeocodeAsync(
            string address,
            string municipality
        );
    }
}
