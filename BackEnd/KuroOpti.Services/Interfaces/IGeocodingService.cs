namespace KuroOpti.Services.Interfaces
{
    public interface IGeocodingService
    {
        bool IsConfigured { get; }
        Task<(decimal Latitude, decimal Longitude)> GeocodeAsync(
            string address,
            string municipality
        );
    }
}
