using System.Text.Json;
using KuroOpti.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace KuroOpti.Services.Implementations
{
    public class GoogleGeocodingService : IGeocodingService
    {
        private const string GoogleGeocodingUrl =
            "https://maps.googleapis.com/maps/api/geocode/json";

        private readonly HttpClient _httpClient;
        private readonly ILogger<GoogleGeocodingService> _logger;
        private readonly string _apiKey;

        public GoogleGeocodingService(
            HttpClient httpClient,
            ILogger<GoogleGeocodingService> logger,
            IConfiguration configuration
        )
        {
            _httpClient = httpClient;
            _logger = logger;
            _apiKey = configuration["Google:GeocodingApiKey"] ?? string.Empty;
        }

        public async Task<(decimal Latitude, decimal Longitude)> GeocodeAsync(
            string address,
            string municipality
        )
        {
            if (string.IsNullOrEmpty(_apiKey))
            {
                _logger.LogWarning("Google GeocodingApiKey not configured — skipping geocoding");
                return (0, 0);
            }

            string normalized = NormalizeAddress(address);
            string query = $"{normalized}, Lithuania";
            string url =
                $"{GoogleGeocodingUrl}?address={Uri.EscapeDataString(query)}&key={_apiKey}&region=lt&language=lt";

            try
            {
                string json = await _httpClient.GetStringAsync(url);
                JsonDocument doc = JsonDocument.Parse(json);
                JsonElement root = doc.RootElement;

                string status = root.GetProperty("status").GetString() ?? string.Empty;

                if (status != "OK")
                {
                    _logger.LogWarning(
                        "Google geocoding failed for: {Query} — status: {Status}",
                        query,
                        status
                    );
                    return (0, 0);
                }

                JsonElement location = root.GetProperty("results")[0]
                    .GetProperty("geometry")
                    .GetProperty("location");

                decimal lat = location.GetProperty("lat").GetDecimal();
                decimal lng = location.GetProperty("lng").GetDecimal();

                return (lat, lng);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Google geocoding threw for: {Query}", query);
                return (0, 0);
            }
        }

        private static string NormalizeAddress(string address)
        {
            string[] parts = address.Split(',', 2);
            if (parts.Length != 2)
                return address;

            string first = parts[0].Trim();
            string second = parts[1].Trim();

            if (ContainsStreetKeyword(first) == false && ContainsStreetKeyword(second))
                return $"{second}, {first}";

            return address;
        }

        private static bool ContainsStreetKeyword(string s)
        {
            return s.Contains(" g.")
                || s.Contains(" pr.")
                || s.Contains(" al.")
                || s.Contains(" pl.")
                || s.Contains(" aplinkl.")
                || s.Contains(" k.");
        }
    }
}
