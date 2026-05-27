using ClosedXML.Excel;
using HtmlAgilityPack;
using KuroOpti.Entities;
using KuroOpti.Repositories;
using KuroOpti.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace KuroOpti.Services.Implementations
{
    public class EnaFuelPriceImporter : IFuelPriceImporter
    {
        private const string EnaPageUrl = "https://www.ena.lt/degalu-kainos-degalinese/";

        private readonly IFuelStationRepository _repository;
        private readonly IGeocodingService _geocodingService;
        private readonly ILogger<EnaFuelPriceImporter> _logger;
        private readonly HttpClient _httpClient;

        public EnaFuelPriceImporter(
            IFuelStationRepository repository,
            IGeocodingService geocodingService,
            ILogger<EnaFuelPriceImporter> logger,
            HttpClient httpClient
        )
        {
            _repository = repository;
            _geocodingService = geocodingService;
            _logger = logger;
            _httpClient = httpClient;
        }

        public async Task ImportAsync()
        {
            _logger.LogInformation("Starting ENA fuel price import");

            string excelUrl = await ScrapeExcelUrlAsync();
            byte[] excelBytes = await DownloadExcelAsync(excelUrl);
            List<FuelStation> stations = ParseExcel(excelBytes);

            _logger.LogInformation("Parsed {Count} fuel stations", stations.Count);

            await _repository.UpsertAllAsync(stations);

            _logger.LogInformation("Import complete — {Count} stations saved", stations.Count);

            await GeocodeStationsAsync();
        }

        private async Task GeocodeStationsAsync()
        {
            List<FuelStation> ungeocoded = await _repository.GetUngeocodedAsync();

            _logger.LogInformation("Geocoding {Count} ungeocoded stations", ungeocoded.Count);

            foreach (FuelStation station in ungeocoded)
            {
                (decimal lat, decimal lng) = await _geocodingService.GeocodeAsync(
                    station.Address,
                    station.Municipality
                );

                if (lat != 0 || lng != 0)
                {
                    await _repository.UpdateCoordinatesAsync(station.Id, lat, lng);
                    _logger.LogInformation(
                        "Geocoded: {Name} → {Lat}, {Lng}",
                        station.Name,
                        lat,
                        lng
                    );
                }

                await Task.Delay(1100); // Nominatim rate limit: 1 request/second
            }

            _logger.LogInformation("Geocoding complete");
        }

        private async Task<string> ScrapeExcelUrlAsync()
        {
            string html = await _httpClient.GetStringAsync(EnaPageUrl);

            HtmlDocument doc = new();
            doc.LoadHtml(html);

            HtmlNode? link = doc.DocumentNode.SelectSingleNode(
                "//a[contains(@href, 'sharepoint.com') and contains(@href, ':x:')]"
            );

            if (link == null)
                throw new InvalidOperationException(
                    "Could not find Excel download link on ENA page — page structure may have changed"
                );

            string href = link.GetAttributeValue("href", string.Empty);

            string downloadUrl = href.Contains('?') ? href + "&download=1" : href + "?download=1";

            _logger.LogInformation("Found Excel URL: {Url}", downloadUrl);
            return downloadUrl;
        }

        private async Task<byte[]> DownloadExcelAsync(string url)
        {
            HttpResponseMessage response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            byte[] data = await response.Content.ReadAsByteArrayAsync();

            string contentType = response.Content.Headers.ContentType?.MediaType ?? "unknown";
            _logger.LogInformation(
                "Downloaded {Bytes} bytes, Content-Type: {ContentType}",
                data.Length,
                contentType
            );

            string tempPath = Path.Combine(Path.GetTempPath(), "ena_debug.xlsx");
            await File.WriteAllBytesAsync(tempPath, data);
            _logger.LogInformation("Raw file saved to: {Path}", tempPath);

            return data;
        }

        private List<FuelStation> ParseExcel(byte[] data)
        {
            List<FuelStation> stations = new();
            DateTime updatedAt = DateTime.UtcNow;

            using MemoryStream stream = new(data);
            using XLWorkbook workbook = new(stream);

            IXLWorksheet sheet = workbook.Worksheets.First();
            _logger.LogInformation(
                "Using sheet: {Sheet}, rows used: {Rows}",
                sheet.Name,
                sheet.RowsUsed().Count()
            );

            int headerRowNumber = FindHeaderRow(sheet);
            if (headerRowNumber < 0)
                throw new InvalidOperationException("Could not find header row in ENA Excel");

            Dictionary<string, int> columns = new(StringComparer.OrdinalIgnoreCase);
            foreach (IXLCell cell in sheet.Row(headerRowNumber).CellsUsed())
                columns[cell.GetString().Trim()] = cell.Address.ColumnNumber;

            _logger.LogInformation(
                "Excel columns found: {Columns}",
                string.Join(", ", columns.Select(kv => $"[{kv.Value}] {kv.Key}"))
            );

            int colName = FindColumn(columns, "Įmonė (Degalinių tinklas)", "Įmonė");
            int colMunicip = FindColumn(columns, "Degalinės vieta (Savivaldybė)", "Savivaldybė");
            int colAddress = FindColumn(columns, "Degalinės vieta (Gyvenvietė, gatvė)", "Adresas");
            int colDiesel = FindColumn(columns, "Dyzelinas");
            int colPetrol = FindColumn(columns, "95 benzinas");
            int colLpg = FindColumn(columns, "SND");

            if (colName < 0)
                throw new InvalidOperationException(
                    "Could not find station name column — check actual Excel column headers in the log above"
                );

            foreach (IXLRow row in sheet.RowsUsed().Skip(headerRowNumber))
            {
                string name = GetString(row, colName);
                if (string.IsNullOrWhiteSpace(name))
                    continue;

                FuelStation station = new()
                {
                    Name = name,
                    Municipality = GetString(row, colMunicip),
                    Address = GetString(row, colAddress),
                    Latitude = 0,
                    Longitude = 0,
                    DieselPrice = GetDecimal(row, colDiesel),
                    PetrolPrice = GetDecimal(row, colPetrol),
                    LpgPrice = GetDecimal(row, colLpg),
                    UpdatedAt = updatedAt,
                };

                stations.Add(station);
            }

            return stations;
        }

        private static int FindHeaderRow(IXLWorksheet sheet)
        {
            foreach (IXLRow row in sheet.RowsUsed())
            {
                string rowText = string.Join(" ", row.CellsUsed().Select(c => c.GetString()));
                if (rowText.Contains("Įmonė") || rowText.Contains("Dyzelinas"))
                    return row.RowNumber();
            }
            return -1;
        }

        private static int FindColumn(Dictionary<string, int> columns, params string[] candidates)
        {
            foreach (string candidate in candidates)
            {
                if (columns.TryGetValue(candidate, out int index))
                    return index;
            }
            return -1;
        }

        private static string GetString(IXLRow row, int col)
        {
            if (col < 0)
                return string.Empty;
            return row.Cell(col).GetString().Trim();
        }

        private static decimal GetDecimal(IXLRow row, int col)
        {
            if (col < 0)
                return 0;
            IXLCell cell = row.Cell(col);
            if (cell.TryGetValue(out decimal value))
                return value;
            if (
                decimal.TryParse(
                    cell.GetString().Trim().Replace(',', '.').Replace(" ", ""),
                    System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture,
                    out decimal parsed
                )
            )
                return parsed;
            return 0;
        }
    }
}
