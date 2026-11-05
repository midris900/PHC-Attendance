using DataAccess.Repository;
using Domain.Core;
using Domain.GHOracleDTO;

namespace PHCAttendanceService.GHService;

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
