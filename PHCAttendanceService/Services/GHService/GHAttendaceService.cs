using DataAccess.Repository;
using Domain.Core;
using Domain.GHOracleDTO;
using Domain.GHOracleModel;
using Domain.HikProModels;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace PHCAttendanceService.Services.GHService;

public class GHAttendaceService
{
    private readonly IDataRepository _repository;

    public GHAttendaceService(IDataRepository repository) => _repository = repository;

    public async Task<TblAttendanceDataReqphcRespDTO> GetGHAttendanceRequests(DateTime date)
    {
        try
        {
            var requests = await _repository.GetGHAttendanceRequests(date);
            if (requests.Count == 0)
                return Error(ConfigClass.DATA_NOT_FOUND, ConfigClass.DATA_NOT_FOUND_MESSAGE);

            return new TblAttendanceDataReqphcRespDTO
            {
                RESPONSE_CODE = ConfigClass.SUCCESS,
                RESPONSE_DESCRPTION = ConfigClass.SUCCESS_MESSAGE,
                lstData = requests
            };
        }
        catch (Exception)
        {
            return Error(ConfigClass.INTERNAL_ERROR, "An error occurred while retrieving GH attendance requests.");
        }
    }

    public async Task<GHAttendanceAccessLogRespDTO> GetHikProAccessLogsByCprsAndDate(
        GHAttendanceAccessLogRequestDTO? request)
    {
        if (request?.Date == null)
            return AccessLogError(ConfigClass.INVALID_REQUEST, ConfigClass.INVALID_REQUEST_MESSAGE);

        var normalizedCprs = request.Cprs?
            .Where(cpr => !string.IsNullOrWhiteSpace(cpr))
            .Select(cpr => cpr.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList() ?? [];

        if (normalizedCprs.Count == 0)
            return AccessLogError(ConfigClass.INVALID_REQUEST, ConfigClass.INVALID_REQUEST_MESSAGE);

        try
        {
            var logs = await _repository.GetHikProAccessLogsByCprsAndDate(normalizedCprs, request.Date.Value);
            if (logs.Count == 0)
                return AccessLogError(ConfigClass.DATA_NOT_FOUND, ConfigClass.DATA_NOT_FOUND_MESSAGE);

            return new GHAttendanceAccessLogRespDTO
            {
                RESPONSE_CODE = ConfigClass.SUCCESS,
                RESPONSE_DESCRPTION = ConfigClass.SUCCESS_MESSAGE,
                lstData = logs
            };
        }
        catch (Exception)
        {
            return AccessLogError(ConfigClass.INTERNAL_ERROR,
                "An error occurred while retrieving GH attendance access logs.");
        }
    }

    public async Task<TblAttendanceDataLogPhcRespDTO> InsertGHAttendanceLogs(List<HikProAccessLog>? accessLogs)
    {
        if (accessLogs == null || accessLogs.Count == 0 || accessLogs.Any(log => log == null))
            return InsertError(ConfigClass.INVALID_REQUEST, ConfigClass.INVALID_REQUEST_MESSAGE);

        try
        {
            var attendanceLogs = accessLogs.Select(log => new TblAttendanceDataLogPhc
            {
                Id = log.LogId,
                EmpId = log.EmpId,
                AccessDateTime = log.AccessDateTime.ToString("yyyy-MM-dd HH:mm:ss.fffffff", CultureInfo.InvariantCulture),
                AccessDate = log.AccessDate?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                AccessTime = log.AccessTime?.ToString("HH:mm:ss.fffffff", CultureInfo.InvariantCulture),
                DevName = log.DevName,
                DevSn = log.DevSn,
                EmpFirstName = log.EmpFirstName,
                EmpLastName = log.EmpLastName
            }).ToList();

            if (attendanceLogs.Any(log => !Validator.TryValidateObject(
                log, new ValidationContext(log), new List<ValidationResult>(), validateAllProperties: true)))
                return InsertError(ConfigClass.INVALID_REQUEST, ConfigClass.INVALID_REQUEST_MESSAGE);

            await _repository.InsertGHAttendanceLogs(attendanceLogs);
            return new TblAttendanceDataLogPhcRespDTO
            {
                RESPONSE_CODE = ConfigClass.SUCCESS,
                RESPONSE_DESCRPTION = ConfigClass.SUCCESS_MESSAGE,
                lstData = attendanceLogs
            };
        }
        catch (Exception)
        {
            return InsertError(ConfigClass.INTERNAL_ERROR,
                "An error occurred while inserting GH attendance logs.");
        }
    }

    private static TblAttendanceDataLogPhcRespDTO InsertError(string code, string description) => new()
    {
        RESPONSE_CODE = code,
        RESPONSE_DESCRPTION = description
    };

    private static GHAttendanceAccessLogRespDTO AccessLogError(string code, string description) => new()
    {
        RESPONSE_CODE = code,
        RESPONSE_DESCRPTION = description
    };

    private static TblAttendanceDataReqphcRespDTO Error(string code, string description) => new()
    {
        RESPONSE_CODE = code,
        RESPONSE_DESCRPTION = description
    };
}
