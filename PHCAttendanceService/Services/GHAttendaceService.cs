using DataAccess.Repository;
using Domain.Core;
using Domain.GHOracleDTO;

namespace PHCAttendanceService.Services;

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

    private static TblAttendanceDataReqphcRespDTO Error(string code, string description) => new()
    {
        RESPONSE_CODE = code,
        RESPONSE_DESCRPTION = description
    };
}
