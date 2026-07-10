using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using ClosedXML.Excel;
using HtmlAgilityPack;
using KuroOpti.Entities;
using KuroOpti.Repositories;
using KuroOpti.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace KuroOpti.Services.Implementations;

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

        EnaLinkInfo linkInfo = await ScrapeExcelLinkAsync();
        ParsedWorkbook? acceptedWorkbook = null;
        List<string> failures = [];

        foreach (string candidateUrl in BuildDownloadCandidates(linkInfo.Url))
        {
            try
            {
                _logger.LogInformation("Trying ENA Excel download URL: {Url}", candidateUrl);

                byte[] excelBytes = await DownloadExcelAsync(candidateUrl);
                ParsedWorkbook parsed = ParseExcel(excelBytes);
                ValidateParsedWorkbook(parsed, linkInfo.AdvertisedDate);

                acceptedWorkbook = parsed;
                break;
            }
            catch (Exception exception)
            {
                failures.Add($"{exception.GetType().Name}: {exception.Message}");

                _logger.LogWarning(exception, "ENA Excel candidate failed: {Url}", candidateUrl);
            }
        }

        if (acceptedWorkbook == null)
        {
            throw new InvalidOperationException(
                "Could not download and validate current ENA Excel data. "
                    + string.Join(" | ", failures)
            );
        }

        _logger.LogInformation(
            "Saving {Count} stations from worksheet {Worksheet}, "
                + "format {Format}, latest source date {LatestDate}",
            acceptedWorkbook.Stations.Count,
            acceptedWorkbook.WorksheetName,
            acceptedWorkbook.Format,
            acceptedWorkbook.LatestSourceDate?.ToString("yyyy-MM-dd") ?? "unknown"
        );

        await _repository.UpsertAllAsync(acceptedWorkbook.Stations);

        _logger.LogInformation(
            "Import complete — {Count} stations saved",
            acceptedWorkbook.Stations.Count
        );

        await GeocodeStationsAsync();
    }

    private async Task<EnaLinkInfo> ScrapeExcelLinkAsync()
    {
        using HttpRequestMessage request = new(HttpMethod.Get, EnaPageUrl);

        AddBrowserHeaders(request);
        AddNoCacheHeaders(request);

        using HttpResponseMessage response = await _httpClient.SendAsync(request);

        response.EnsureSuccessStatusCode();

        string html = await response.Content.ReadAsStringAsync();

        HtmlDocument document = new();
        document.LoadHtml(html);

        HtmlNodeCollection? anchors = document.DocumentNode.SelectNodes("//a[@href]");

        if (anchors == null)
        {
            throw new InvalidOperationException("ENA page does not contain any links");
        }

        HtmlNode? link = anchors.FirstOrDefault(anchor =>
        {
            string href = WebUtility.HtmlDecode(anchor.GetAttributeValue("href", string.Empty));

            string text = NormalizeForMatch(WebUtility.HtmlDecode(anchor.InnerText));

            return href.Contains("sharepoint.com", StringComparison.OrdinalIgnoreCase)
                && href.Contains(":x:", StringComparison.OrdinalIgnoreCase)
                && text.Contains("naujausios degalu kainos", StringComparison.OrdinalIgnoreCase);
        });

        link ??= anchors.FirstOrDefault(anchor =>
        {
            string href = WebUtility.HtmlDecode(anchor.GetAttributeValue("href", string.Empty));

            return href.Contains("sharepoint.com", StringComparison.OrdinalIgnoreCase)
                && href.Contains(":x:", StringComparison.OrdinalIgnoreCase);
        });

        if (link == null)
        {
            throw new InvalidOperationException(
                "Could not find the SharePoint Excel link on the ENA page"
            );
        }

        string linkUrl = WebUtility.HtmlDecode(link.GetAttributeValue("href", string.Empty));

        DateTime? advertisedDate = ExtractDate(link.InnerText) ?? ExtractLatestDateFromPage(html);

        _logger.LogInformation(
            "Found ENA Excel link {Url}; advertised date {Date}",
            linkUrl,
            advertisedDate?.ToString("yyyy-MM-dd") ?? "unknown"
        );

        return new EnaLinkInfo(linkUrl, advertisedDate);
    }

    private static IEnumerable<string> BuildDownloadCandidates(string sharePointUrl)
    {
        long cacheBust = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        string first = SetQueryParameter(sharePointUrl, "download", "1");

        first = SetQueryParameter(first, "web", "0");

        first = SetQueryParameter(
            first,
            "cacheBust",
            cacheBust.ToString(CultureInfo.InvariantCulture)
        );

        string second = SetQueryParameter(sharePointUrl, "download", "1");

        second = SetQueryParameter(
            second,
            "cacheBust",
            cacheBust.ToString(CultureInfo.InvariantCulture)
        );

        return new[] { first, second }.Distinct(StringComparer.OrdinalIgnoreCase);
    }

    private async Task<byte[]> DownloadExcelAsync(string url)
    {
        using HttpRequestMessage request = new(HttpMethod.Get, url);

        AddBrowserHeaders(request);
        AddNoCacheHeaders(request);

        using HttpResponseMessage response = await _httpClient.SendAsync(
            request,
            HttpCompletionOption.ResponseHeadersRead
        );

        response.EnsureSuccessStatusCode();

        byte[] data = await response.Content.ReadAsByteArrayAsync();

        string contentType = response.Content.Headers.ContentType?.MediaType ?? "unknown";

        Uri? finalUrl = response.RequestMessage?.RequestUri;

        _logger.LogInformation(
            "Downloaded {Bytes} bytes; Content-Type: {ContentType}; " + "final URL: {FinalUrl}",
            data.Length,
            contentType,
            finalUrl
        );

        bool looksLikeHtml =
            contentType.Contains("text/html", StringComparison.OrdinalIgnoreCase)
            || StartsWithHtml(data);

        bool hasZipSignature = data.Length >= 4 && data[0] == 0x50 && data[1] == 0x4B;

        if (looksLikeHtml || !hasZipSignature)
        {
            throw new InvalidDataException(
                "SharePoint did not return a valid XLSX file. "
                    + $"Content-Type: {contentType}; final URL: {finalUrl}; "
                    + $"preview: {GetTextPreview(data)}"
            );
        }

        string tempPath = Path.Combine(Path.GetTempPath(), "ena_debug.xlsx");

        await File.WriteAllBytesAsync(tempPath, data);

        _logger.LogInformation("Downloaded Excel saved for inspection: {Path}", tempPath);

        return data;
    }

    private ParsedWorkbook ParseExcel(byte[] data)
    {
        using MemoryStream stream = new(data);
        using XLWorkbook workbook = new(stream);

        ExcelLayout layout = DetectExcelLayout(workbook);

        _logger.LogInformation(
            "Using worksheet {Worksheet}; header row {HeaderRow}; " + "format {Format}",
            layout.Sheet.Name,
            layout.HeaderRowNumber,
            layout.Format
        );

        _logger.LogInformation(
            "Excel columns found: {Columns}",
            string.Join(", ", layout.Columns.Select(column => $"[{column.Value}] {column.Key}"))
        );

        ParseResult parseResult = layout.Format switch
        {
            EnaExcelFormat.Long => ParseLongFormat(layout),

            EnaExcelFormat.Wide => ParseWideFormat(layout),

            _ => throw new InvalidOperationException("Unsupported ENA Excel format"),
        };

        if (parseResult.Stations.Count == 0)
        {
            throw new InvalidDataException("ENA Excel was opened, but no stations were parsed");
        }

        DateTime? latestSourceDate =
            parseResult.LatestSourceDate ?? FindLatestDateAnywhere(workbook);

        LogParseSummary(parseResult.Stations, latestSourceDate);

        return new ParsedWorkbook(
            parseResult.Stations,
            latestSourceDate,
            layout.Sheet.Name,
            layout.Format
        );
    }

    private ExcelLayout DetectExcelLayout(XLWorkbook workbook)
    {
        foreach (IXLWorksheet sheet in workbook.Worksheets)
        {
            _logger.LogInformation(
                "Checking worksheet {Worksheet}; rows used: {Rows}",
                sheet.Name,
                sheet.RowsUsed().Count()
            );

            foreach (IXLRow row in sheet.RowsUsed())
            {
                Dictionary<string, int> columns = ReadHeaderColumns(row);

                if (columns.Count < 3)
                {
                    continue;
                }

                bool hasCompany =
                    FindColumn(columns, "Įmonė", "Degalinių tinklas", "Pardavėjas") >= 0;

                bool hasMunicipality = FindColumn(columns, "Savivaldybė") >= 0;

                bool hasAddress =
                    FindColumn(columns, "Adresas", "Gyvenvietė, gatvė", "Degalinės vieta") >= 0;

                bool hasFuelType =
                    FindColumn(columns, "Degalų tipas", "Degalų rūšis", "Kuro tipas", "Kuro rūšis")
                    >= 0;

                bool hasPrice =
                    FindColumn(columns, "Kaina (EUR/l)", "Kaina EUR/l", "Kaina, EUR/l", "Kaina")
                    >= 0;

                if (hasCompany && hasMunicipality && hasAddress && hasFuelType && hasPrice)
                {
                    return new ExcelLayout(sheet, row.RowNumber(), EnaExcelFormat.Long, columns);
                }

                int wideFuelColumns = 0;

                if (FindColumn(columns, "Dyzelinas") >= 0)
                {
                    wideFuelColumns++;
                }

                if (FindColumn(columns, "95 benzinas", "Benzinas 95", "A95") >= 0)
                {
                    wideFuelColumns++;
                }

                if (FindColumn(columns, "SND", "LPG") >= 0)
                {
                    wideFuelColumns++;
                }

                if (hasCompany && hasMunicipality && hasAddress && wideFuelColumns >= 2)
                {
                    return new ExcelLayout(sheet, row.RowNumber(), EnaExcelFormat.Wide, columns);
                }
            }
        }

        throw new InvalidOperationException(
            "Could not find a supported ENA fuel-price table " + "in any worksheet"
        );
    }

    private ParseResult ParseLongFormat(ExcelLayout layout)
    {
        Dictionary<string, int> columns = layout.Columns;

        int colName = RequireColumn(columns, "Įmonė", "Degalinių tinklas", "Pardavėjas");

        int colMunicipality = RequireColumn(columns, "Savivaldybė");

        int colAddress = RequireColumn(columns, "Adresas", "Gyvenvietė, gatvė", "Degalinės vieta");

        int colFuelType = RequireColumn(
            columns,
            "Degalų tipas",
            "Degalų rūšis",
            "Kuro tipas",
            "Kuro rūšis"
        );

        int colPrice = RequireColumn(
            columns,
            "Kaina (EUR/l)",
            "Kaina EUR/l",
            "Kaina, EUR/l",
            "Kaina"
        );

        int colSubmittedDate = FindColumn(columns, "Pateikimo data", "Duomenų data", "Data");

        Dictionary<string, FuelStation> stations = new(StringComparer.OrdinalIgnoreCase);

        Dictionary<string, DateTime> latestFuelDates = new(StringComparer.OrdinalIgnoreCase);

        DateTime? latestSourceDate = null;

        foreach (
            IXLRow row in layout
                .Sheet.RowsUsed()
                .Where(row => row.RowNumber() > layout.HeaderRowNumber)
        )
        {
            string name = GetString(row, colName);
            string municipality = GetString(row, colMunicipality);
            string address = GetString(row, colAddress);
            string rawFuelType = GetString(row, colFuelType);
            decimal price = GetDecimal(row, colPrice);

            if (
                string.IsNullOrWhiteSpace(name)
                || string.IsNullOrWhiteSpace(address)
                || string.IsNullOrWhiteSpace(rawFuelType)
                || price <= 0
            )
            {
                continue;
            }

            FuelKind? fuelKind = GetFuelKind(rawFuelType);

            if (fuelKind == null)
            {
                continue;
            }

            DateTime? submittedDate = GetDateTime(row, colSubmittedDate);

            if (
                submittedDate.HasValue
                && (!latestSourceDate.HasValue || submittedDate.Value > latestSourceDate.Value)
            )
            {
                latestSourceDate = submittedDate.Value;
            }

            string stationKey = BuildStationKey(name, municipality, address);

            if (!stations.TryGetValue(stationKey, out FuelStation? station))
            {
                station = new FuelStation
                {
                    Name = NormalizeWhitespace(name),
                    Municipality = NormalizeWhitespace(municipality),
                    Address = NormalizeWhitespace(address),
                    Latitude = 0,
                    Longitude = 0,
                    PetrolPrice = 0,
                    DieselPrice = 0,
                    LpgPrice = 0,
                    UpdatedAt = submittedDate ?? DateTime.UtcNow,
                };

                stations[stationKey] = station;
            }

            string fuelDateKey = $"{stationKey}|{fuelKind.Value}";

            if (
                submittedDate.HasValue
                && latestFuelDates.TryGetValue(fuelDateKey, out DateTime existingDate)
                && existingDate > submittedDate.Value
            )
            {
                continue;
            }

            if (submittedDate.HasValue)
            {
                latestFuelDates[fuelDateKey] = submittedDate.Value;
            }

            SetFuelPrice(station, fuelKind.Value, price);

            if (submittedDate.HasValue && submittedDate.Value > station.UpdatedAt)
            {
                station.UpdatedAt = submittedDate.Value;
            }
        }

        return new ParseResult(stations.Values.ToList(), latestSourceDate);
    }

    private ParseResult ParseWideFormat(ExcelLayout layout)
    {
        Dictionary<string, int> columns = layout.Columns;

        int colName = RequireColumn(columns, "Įmonė", "Degalinių tinklas", "Pardavėjas");

        int colMunicipality = RequireColumn(columns, "Savivaldybė");

        int colAddress = RequireColumn(columns, "Adresas", "Gyvenvietė, gatvė", "Degalinės vieta");

        int colDiesel = FindColumn(columns, "Dyzelinas");

        int colPetrol = FindColumn(columns, "95 benzinas", "Benzinas 95", "A95");

        int colLpg = FindColumn(columns, "SND", "LPG");

        int colSubmittedDate = FindColumn(columns, "Pateikimo data", "Duomenų data", "Data");

        Dictionary<string, FuelStation> stations = new(StringComparer.OrdinalIgnoreCase);

        DateTime? latestSourceDate = null;

        foreach (
            IXLRow row in layout
                .Sheet.RowsUsed()
                .Where(row => row.RowNumber() > layout.HeaderRowNumber)
        )
        {
            string name = GetString(row, colName);
            string municipality = GetString(row, colMunicipality);
            string address = GetString(row, colAddress);

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(address))
            {
                continue;
            }

            DateTime? submittedDate = GetDateTime(row, colSubmittedDate);

            if (
                submittedDate.HasValue
                && (!latestSourceDate.HasValue || submittedDate.Value > latestSourceDate.Value)
            )
            {
                latestSourceDate = submittedDate.Value;
            }

            string stationKey = BuildStationKey(name, municipality, address);

            stations[stationKey] = new FuelStation
            {
                Name = NormalizeWhitespace(name),
                Municipality = NormalizeWhitespace(municipality),
                Address = NormalizeWhitespace(address),
                Latitude = 0,
                Longitude = 0,
                PetrolPrice = GetDecimal(row, colPetrol),
                DieselPrice = GetDecimal(row, colDiesel),
                LpgPrice = GetDecimal(row, colLpg),
                UpdatedAt = submittedDate ?? DateTime.UtcNow,
            };
        }

        return new ParseResult(stations.Values.ToList(), latestSourceDate);
    }

    private void ValidateParsedWorkbook(ParsedWorkbook parsed, DateTime? advertisedDate)
    {
        if (parsed.Stations.Count == 0)
        {
            throw new InvalidDataException("The downloaded workbook contains no usable stations");
        }

        bool hasAnyPrice = parsed.Stations.Any(station =>
            station.PetrolPrice > 0 || station.DieselPrice > 0 || station.LpgPrice > 0
        );

        if (!hasAnyPrice)
        {
            throw new InvalidDataException(
                "The workbook contains stations, but no usable fuel prices"
            );
        }

        if (!advertisedDate.HasValue)
        {
            _logger.LogWarning(
                "Could not read the advertised date from the ENA page; "
                    + "source freshness cannot be compared"
            );
            return;
        }

        if (!parsed.LatestSourceDate.HasValue)
        {
            throw new InvalidDataException(
                $"ENA page advertises data for "
                    + $"{advertisedDate.Value:yyyy-MM-dd}, but the downloaded "
                    + "workbook contains no readable source date. "
                    + "Import was stopped to avoid writing stale prices."
            );
        }

        if (parsed.LatestSourceDate.Value.Date < advertisedDate.Value.Date)
        {
            throw new InvalidDataException(
                $"Downloaded ENA workbook is stale. "
                    + $"ENA page advertises "
                    + $"{advertisedDate.Value:yyyy-MM-dd}, but the workbook's "
                    + $"latest source date is "
                    + $"{parsed.LatestSourceDate.Value:yyyy-MM-dd}. "
                    + "No database changes were made."
            );
        }
    }

    private sealed record GeocodingResult(
        int StationId,
        string StationName,
        decimal Latitude,
        decimal Longitude
    );

    private async Task GeocodeStationsAsync()
    {
        if (!_geocodingService.IsConfigured)
        {
            _logger.LogWarning("Geocoding skipped because Google API key is not configured");

            return;
        }

        List<FuelStation> ungeocoded = await _repository.GetUngeocodedAsync();

        if (ungeocoded.Count == 0)
        {
            _logger.LogInformation("No ungeocoded stations found");

            return;
        }

        const int maxConcurrentRequests = 8;

        _logger.LogInformation(
            "Geocoding {Count} stations with concurrency {Concurrency}",
            ungeocoded.Count,
            maxConcurrentRequests
        );

        using SemaphoreSlim semaphore = new(maxConcurrentRequests);

        int completedCount = 0;

        Task<GeocodingResult>[] tasks = ungeocoded
            .Select(async station =>
            {
                await semaphore.WaitAsync();

                try
                {
                    (decimal lat, decimal lng) = await _geocodingService.GeocodeAsync(
                        station.Address,
                        station.Municipality
                    );

                    int completed = Interlocked.Increment(ref completedCount);

                    if (completed % 25 == 0 || completed == ungeocoded.Count)
                    {
                        _logger.LogInformation(
                            "Geocoding progress: {Completed}/{Total}",
                            completed,
                            ungeocoded.Count
                        );
                    }

                    return new GeocodingResult(station.Id, station.Name, lat, lng);
                }
                catch (Exception exception)
                {
                    _logger.LogWarning(
                        exception,
                        "Failed to geocode station {StationId}: {Name}",
                        station.Id,
                        station.Name
                    );

                    return new GeocodingResult(station.Id, station.Name, 0, 0);
                }
                finally
                {
                    semaphore.Release();
                }
            })
            .ToArray();

        GeocodingResult[] results = await Task.WhenAll(tasks);

        GeocodingResult[] successfulResults = results
            .Where(result => result.Latitude != 0 && result.Longitude != 0)
            .ToArray();

        /*
         * DB atnaujiname nuosekliai.
         * Tas pats scoped DbContext nėra thread-safe.
         */
        foreach (GeocodingResult result in successfulResults)
        {
            await _repository.UpdateCoordinatesAsync(
                result.StationId,
                result.Latitude,
                result.Longitude
            );
        }

        _logger.LogInformation(
            "Geocoding complete. Successful: {Successful}, failed: {Failed}",
            successfulResults.Length,
            results.Length - successfulResults.Length
        );
    }

    private void LogParseSummary(
        IReadOnlyCollection<FuelStation> stations,
        DateTime? latestSourceDate
    )
    {
        _logger.LogInformation(
            "Parsed {Count} unique stations; latest source date: {Date}",
            stations.Count,
            latestSourceDate?.ToString("yyyy-MM-dd") ?? "unknown"
        );

        _logger.LogInformation(
            "Petrol: {Count} stations; minimum {Minimum}",
            stations.Count(station => station.PetrolPrice > 0),
            stations
                .Where(station => station.PetrolPrice > 0)
                .Select(station => station.PetrolPrice)
                .DefaultIfEmpty(0)
                .Min()
        );

        _logger.LogInformation(
            "Diesel: {Count} stations; minimum {Minimum}",
            stations.Count(station => station.DieselPrice > 0),
            stations
                .Where(station => station.DieselPrice > 0)
                .Select(station => station.DieselPrice)
                .DefaultIfEmpty(0)
                .Min()
        );

        _logger.LogInformation(
            "LPG: {Count} stations; minimum {Minimum}",
            stations.Count(station => station.LpgPrice > 0),
            stations
                .Where(station => station.LpgPrice > 0)
                .Select(station => station.LpgPrice)
                .DefaultIfEmpty(0)
                .Min()
        );
    }

    private static Dictionary<string, int> ReadHeaderColumns(IXLRow row)
    {
        Dictionary<string, int> columns = new(StringComparer.OrdinalIgnoreCase);

        foreach (IXLCell cell in row.CellsUsed())
        {
            string normalizedHeader = NormalizeForMatch(cell.GetFormattedString());

            if (!string.IsNullOrWhiteSpace(normalizedHeader))
            {
                columns[normalizedHeader] = cell.Address.ColumnNumber;
            }
        }

        return columns;
    }

    private static int RequireColumn(Dictionary<string, int> columns, params string[] candidates)
    {
        int column = FindColumn(columns, candidates);

        if (column >= 0)
        {
            return column;
        }

        throw new InvalidOperationException(
            $"Required Excel column not found: " + $"{string.Join(" / ", candidates)}"
        );
    }

    private static int FindColumn(Dictionary<string, int> columns, params string[] candidates)
    {
        string[] normalizedCandidates = candidates
            .Select(NormalizeForMatch)
            .Where(candidate => !string.IsNullOrWhiteSpace(candidate))
            .ToArray();

        foreach (string candidate in normalizedCandidates)
        {
            if (columns.TryGetValue(candidate, out int exactColumn))
            {
                return exactColumn;
            }
        }

        foreach (KeyValuePair<string, int> column in columns)
        {
            foreach (string candidate in normalizedCandidates)
            {
                if (
                    column.Key.Contains(candidate, StringComparison.OrdinalIgnoreCase)
                    || candidate.Contains(column.Key, StringComparison.OrdinalIgnoreCase)
                )
                {
                    return column.Value;
                }
            }
        }

        return -1;
    }

    private static string GetString(IXLRow row, int column)
    {
        if (column < 0)
        {
            return string.Empty;
        }

        return NormalizeWhitespace(row.Cell(column).GetFormattedString());
    }

    private static decimal GetDecimal(IXLRow row, int column)
    {
        if (column < 0)
        {
            return 0;
        }

        IXLCell cell = row.Cell(column);

        if (cell.TryGetValue(out decimal numericValue))
        {
            return numericValue;
        }

        string text = cell.GetFormattedString()
            .Replace(' ', ' ')
            .Replace("EUR", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("€", string.Empty)
            .Replace(" ", string.Empty)
            .Trim()
            .Replace(',', '.');

        return decimal.TryParse(
            text,
            NumberStyles.Number,
            CultureInfo.InvariantCulture,
            out decimal parsed
        )
            ? parsed
            : 0;
    }

    private static DateTime? GetDateTime(IXLRow row, int column)
    {
        if (column < 0)
        {
            return null;
        }

        return TryReadDate(row.Cell(column));
    }

    private static DateTime? TryReadDate(IXLCell cell)
    {
        if (cell.TryGetValue(out DateTime dateValue))
        {
            return dateValue;
        }

        string text = NormalizeWhitespace(cell.GetFormattedString());

        string[] formats =
        [
            "yyyy-MM-dd",
            "yyyy.MM.dd",
            "yyyy/MM/dd",
            "dd.MM.yyyy",
            "dd-MM-yyyy",
            "M/d/yyyy",
            "MM/dd/yyyy",
        ];

        if (
            DateTime.TryParseExact(
                text,
                formats,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out DateTime exactDate
            )
        )
        {
            return exactDate;
        }

        if (
            DateTime.TryParse(
                text,
                CultureInfo.GetCultureInfo("lt-LT"),
                DateTimeStyles.None,
                out DateTime lithuanianDate
            )
        )
        {
            return lithuanianDate;
        }

        return null;
    }

    private static DateTime? FindLatestDateAnywhere(XLWorkbook workbook)
    {
        DateTime? latest = null;

        foreach (IXLWorksheet sheet in workbook.Worksheets)
        {
            foreach (IXLCell cell in sheet.CellsUsed())
            {
                DateTime? date = TryReadDate(cell);

                if (!date.HasValue || date.Value.Year < 2020 || date.Value.Year > 2100)
                {
                    continue;
                }

                if (!latest.HasValue || date.Value > latest.Value)
                {
                    latest = date.Value;
                }
            }
        }

        return latest;
    }

    private static FuelKind? GetFuelKind(string value)
    {
        string normalized = NormalizeForMatch(value);

        if (normalized == "snd" || normalized.Contains("lpg") || normalized.Contains("suskystint"))
        {
            return FuelKind.Lpg;
        }

        if (normalized.Contains("dyzel"))
        {
            return FuelKind.Diesel;
        }

        if (
            normalized.Contains("benzin")
            && (normalized.Contains("95") || normalized.Contains("a95") || normalized == "benzinas")
        )
        {
            return FuelKind.Petrol;
        }

        return null;
    }

    private static void SetFuelPrice(FuelStation station, FuelKind fuelKind, decimal price)
    {
        switch (fuelKind)
        {
            case FuelKind.Petrol:
                station.PetrolPrice = price;
                break;

            case FuelKind.Diesel:
                station.DieselPrice = price;
                break;

            case FuelKind.Lpg:
                station.LpgPrice = price;
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(fuelKind), fuelKind, null);
        }
    }

    private static string BuildStationKey(string name, string municipality, string address)
    {
        return string.Join(
            "|",
            NormalizeForMatch(name),
            NormalizeForMatch(municipality),
            NormalizeForMatch(address)
        );
    }

    private static string NormalizeWhitespace(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        return string.Join(
                " ",
                value.Replace(' ', ' ').Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries)
            )
            .Trim();
    }

    private static string NormalizeForMatch(string value)
    {
        string normalized = NormalizeWhitespace(value)
            .ToLowerInvariant()
            .Normalize(NormalizationForm.FormD);

        StringBuilder builder = new();

        foreach (char character in normalized)
        {
            UnicodeCategory category = CharUnicodeInfo.GetUnicodeCategory(character);

            if (category != UnicodeCategory.NonSpacingMark)
            {
                builder.Append(character);
            }
        }

        string withoutDiacritics = builder.ToString().Normalize(NormalizationForm.FormC);

        return Regex.Replace(withoutDiacritics, @"[^\p{L}\p{Nd}]+", " ").Trim();
    }

    private static DateTime? ExtractDate(string value)
    {
        Match match = Regex.Match(value, @"\b(?<date>\d{4}-\d{2}-\d{2})\b");

        if (
            match.Success
            && DateTime.TryParseExact(
                match.Groups["date"].Value,
                "yyyy-MM-dd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out DateTime date
            )
        )
        {
            return date;
        }

        return null;
    }

    private static DateTime? ExtractLatestDateFromPage(string html)
    {
        MatchCollection matches = Regex.Matches(html, @"\b(?<date>\d{4}-\d{2}-\d{2})\b");

        List<DateTime> dates = [];

        foreach (Match match in matches)
        {
            if (
                DateTime.TryParseExact(
                    match.Groups["date"].Value,
                    "yyyy-MM-dd",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out DateTime date
                )
            )
            {
                dates.Add(date);
            }
        }

        return dates.Count == 0 ? null : dates.Max();
    }

    private static string SetQueryParameter(string url, string key, string value)
    {
        UriBuilder builder = new(url);

        List<string> queryParts = builder
            .Query.TrimStart('?')
            .Split('&', StringSplitOptions.RemoveEmptyEntries)
            .Where(part =>
            {
                string rawKey = part.Split('=', 2)[0];

                string decodedKey = Uri.UnescapeDataString(rawKey);

                return !string.Equals(decodedKey, key, StringComparison.OrdinalIgnoreCase);
            })
            .ToList();

        queryParts.Add($"{Uri.EscapeDataString(key)}=" + $"{Uri.EscapeDataString(value)}");

        builder.Query = string.Join("&", queryParts);

        return builder.Uri.AbsoluteUri;
    }

    private static void AddBrowserHeaders(HttpRequestMessage request)
    {
        request.Headers.UserAgent.ParseAdd(
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) "
                + "AppleWebKit/537.36 Chrome/149 Safari/537.36"
        );

        request.Headers.Accept.Add(
            new MediaTypeWithQualityHeaderValue(
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
            )
        );

        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/octet-stream"));

        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html", 0.5));
    }

    private static void AddNoCacheHeaders(HttpRequestMessage request)
    {
        request.Headers.CacheControl = new CacheControlHeaderValue
        {
            NoCache = true,
            NoStore = true,
            MaxAge = TimeSpan.Zero,
        };

        request.Headers.Pragma.ParseAdd("no-cache");
    }

    private static bool StartsWithHtml(byte[] data)
    {
        if (data.Length == 0)
        {
            return false;
        }

        string preview = Encoding
            .UTF8.GetString(data, 0, Math.Min(data.Length, 200))
            .TrimStart('\uFEFF', ' ', '\r', '\n', '\t')
            .ToLowerInvariant();

        return preview.StartsWith("<!doctype html")
            || preview.StartsWith("<html")
            || preview.StartsWith("<");
    }

    private static string GetTextPreview(byte[] data)
    {
        if (data.Length == 0)
        {
            return "<empty response>";
        }

        string preview = Encoding.UTF8.GetString(data, 0, Math.Min(data.Length, 300));

        return Regex.Replace(preview, @"\s+", " ");
    }

    private enum EnaExcelFormat
    {
        Long,
        Wide,
    }

    private enum FuelKind
    {
        Petrol,
        Diesel,
        Lpg,
    }

    private sealed record EnaLinkInfo(string Url, DateTime? AdvertisedDate);

    private sealed record ExcelLayout(
        IXLWorksheet Sheet,
        int HeaderRowNumber,
        EnaExcelFormat Format,
        Dictionary<string, int> Columns
    );

    private sealed record ParseResult(List<FuelStation> Stations, DateTime? LatestSourceDate);

    private sealed record ParsedWorkbook(
        List<FuelStation> Stations,
        DateTime? LatestSourceDate,
        string WorksheetName,
        EnaExcelFormat Format
    );
}
