# GHAttendanceConsole

Runs the GH attendance integration once:

1. Gets attendance requests for today's date from `api/GHAttendace`.
2. Sends the distinct CPRs and the same date to `api/GHAttendace/hikpro`.
3. Sends the returned access logs to `api/GHAttendace/postlogs`.
4. Appends the returned `RowsInserted` and completion timestamp to `Logs/yyyy-MM-dd_logs.txt` beside the executable.

Configure `ApiBaseUrl`, optional `ApiKey`, and `TimeZoneId` in `appsettings.json`. The defaults use `http://localhost:5012/` and Bahrain time (`Asia/Bahrain`). `GH_ATTENDANCE_API_BASE_URL` overrides the configured API URL.

Run from the repository root:

```sh
dotnet run --project GHAttendanceConsole/GHAttendanceConsole.csproj
```

The app exits with code 0 after a successful run, including when no data is available or every log already exists. It logs failures and exits with code 1 when configuration, HTTP requests, or API responses fail.
