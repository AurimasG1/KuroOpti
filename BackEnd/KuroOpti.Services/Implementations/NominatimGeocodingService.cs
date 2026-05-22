using System.Text.Json;
using KuroOpti.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace KuroOpti.Services.Implementations
{
	public class NominatimGeocodingService : IGeocodingService
	{
		private const string NominatimUrl = "https://nominatim.openstreetmap.org/search";

		private readonly HttpClient _httpClient;
		private readonly ILogger<NominatimGeocodingService> _logger;

		public NominatimGeocodingService(HttpClient httpClient, ILogger<NominatimGeocodingService> logger)
		{
			_httpClient = httpClient;
			_httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("KuroOpti/1.0");
			_logger = logger;
		}

		public async Task<(decimal Latitude, decimal Longitude)> GeocodeAsync(string address, string municipality)
		{
			string query = $"{address}, {municipality}, Lithuania";
			string url = $"{NominatimUrl}?q={Uri.EscapeDataString(query)}&format=json&limit=1";

			try
			{
				string json = await _httpClient.GetStringAsync(url);
				JsonDocument doc = JsonDocument.Parse(json);
				JsonElement root = doc.RootElement;

				if (root.GetArrayLength() == 0)
				{
					_logger.LogWarning("Nominatim found no result for: {Query}", query);
					return (0, 0);
				}

				JsonElement first = root[0];
				decimal lat = decimal.Parse(first.GetProperty("lat").GetString()!, System.Globalization.CultureInfo.InvariantCulture);
				decimal lng = decimal.Parse(first.GetProperty("lon").GetString()!, System.Globalization.CultureInfo.InvariantCulture);

				return (lat, lng);
			}
			catch (Exception ex)
			{
				_logger.LogWarning(ex, "Geocoding failed for: {Query}", query);
				return (0, 0);
			}
		}
	}
}
