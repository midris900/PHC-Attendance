namespace GHAttendanceConsole;

public class AppOptions
{
    public string ApiBaseUrl { get; set; } = "http://localhost:5012/";
    public string? ApiKey { get; set; }
    public string TimeZoneId { get; set; } = "Asia/Bahrain";
}
