using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json;
using Domain.Core;
using Domain.GHOracleDTO;
using GHAttendanceConsole;

var logDirectory = Path.Combine(AppContext.BaseDirectory, "Logs");
var timeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Bahrain");
var rowsInserted = 0;
var jsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);

try
{
    var settingsPath = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
    await using var settingsStream = File.OpenRead(settingsPath);
    var options = await JsonSerializer.DeserializeAsync<AppOptions>(settingsStream, jsonOptions)
        ?? throw new InvalidOperationException("Console application settings are invalid.");
    timeZone = TimeZoneInfo.FindSystemTimeZoneById(options.TimeZoneId);
    // Capture once so every request uses the same date even if the run crosses midnight.
    var today = TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, timeZone).Date;
    var date = today.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
    var configuredBaseUrl = Environment.GetEnvironmentVariable("GH_ATTENDANCE_API_BASE_URL")
        ?? options.ApiBaseUrl;
    if (!Uri.TryCreate(configuredBaseUrl, UriKind.Absolute, out var baseUri)
        || (baseUri.Scheme != Uri.UriSchemeHttp && baseUri.Scheme != Uri.UriSchemeHttps))
        throw new InvalidOperationException("ApiBaseUrl must be a valid HTTP or HTTPS URL.");

    using var httpClient = new HttpClient
    {
        BaseAddress = new Uri(baseUri.AbsoluteUri.TrimEnd('/') + "/")
    };
    if (!string.IsNullOrWhiteSpace(options.ApiKey))
        httpClient.DefaultRequestHeaders.Add("x-api-key", options.ApiKey);

    using var requestResponse = await httpClient.GetAsync($"api/GHAttendace?date={date}");
    requestResponse.EnsureSuccessStatusCode();
    var requests = await requestResponse.Content.ReadFromJsonAsync<TblAttendanceDataReqphcRespDTO>(jsonOptions)
        ?? throw new InvalidOperationException("GH attendance requests returned an empty response.");
    if (requests.RESPONSE_CODE == ConfigClass.DATA_NOT_FOUND)
        return await CompleteAsync("No GH attendance requests were available.");
    EnsureSuccess(requests.RESPONSE_CODE, requests.RESPONSE_DESCRPTION, "Get GH attendance requests");

    var cprs = requests.lstData?
        .Select(request => request.TadrCpr)
        .Where(cpr => !string.IsNullOrWhiteSpace(cpr))
        .Select(cpr => cpr!.Trim())
        .Distinct(StringComparer.OrdinalIgnoreCase)
        .ToList() ?? [];
    if (cprs.Count == 0)
        return await CompleteAsync("No CPRs were available in the GH attendance requests.");

    var lookup = new GHAttendanceAccessLogRequestDTO { Cprs = cprs, Date = today };
    using var lookupResponse = await httpClient.PostAsJsonAsync("api/GHAttendace/hikpro", lookup);
    lookupResponse.EnsureSuccessStatusCode();
    var logs = await lookupResponse.Content.ReadFromJsonAsync<GHAttendanceAccessLogRespDTO>(jsonOptions)
        ?? throw new InvalidOperationException("HikPro access logs returned an empty response.");
    if (logs.RESPONSE_CODE == ConfigClass.DATA_NOT_FOUND)
        return await CompleteAsync("No HikPro access logs were available for the requested CPRs and date.");
    EnsureSuccess(logs.RESPONSE_CODE, logs.RESPONSE_DESCRPTION, "Get HikPro access logs");
    if (logs.lstData is not { Count: > 0 })
        return await CompleteAsync("No HikPro access logs were available for the requested CPRs and date.");

    using var insertResponse = await httpClient.PostAsJsonAsync("api/GHAttendace/postlogs", logs.lstData);
    insertResponse.EnsureSuccessStatusCode();
    var result = await insertResponse.Content.ReadFromJsonAsync<TblAttendanceDataLogPhcRespDTO>(jsonOptions)
        ?? throw new InvalidOperationException("Insert GH attendance logs returned an empty response.");
    EnsureSuccess(result.RESPONSE_CODE, result.RESPONSE_DESCRPTION, "Insert GH attendance logs");
    rowsInserted = result.RowsInserted;
    return await CompleteAsync($"Fetched {logs.lstData.Count} and inserted {rowsInserted} GH attendance log(s).");
}
catch (Exception ex)
{
    Console.Error.WriteLine($"GH attendance integration failed: {ex.Message}");
    try
    {
        await WriteLogAsync("Failed", ex.Message);
    }
    catch (Exception logException)
    {
        Console.Error.WriteLine($"Unable to write the log file: {logException.Message}");
    }
    return 1;
}

static void EnsureSuccess(string? code, string? description, string operation)
{
    if (code != ConfigClass.SUCCESS)
        throw new InvalidOperationException($"{operation} failed: {description ?? "Unknown API error"}");
}

async Task<int> CompleteAsync(string message)
{
    await WriteLogAsync("Completed", message);
    Console.WriteLine(message);
    return 0;
}

async Task WriteLogAsync(string status, string message)
{
    Directory.CreateDirectory(logDirectory);
    var completedAt = TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, timeZone);
    var logPath = Path.Combine(logDirectory, $"{completedAt:yyyy-MM-dd}_logs.txt");
    var line = $"{completedAt:yyyy-MM-dd HH:mm:ss zzz} Status={status} RowsInserted={rowsInserted} "
        + $"Message={message.Replace('\r', ' ').Replace('\n', ' ')}{Environment.NewLine}";
    await File.AppendAllTextAsync(logPath, line);
}
