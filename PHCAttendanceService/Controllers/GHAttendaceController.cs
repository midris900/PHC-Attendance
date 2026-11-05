using Domain.GHOracleDTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using PHCAttendanceService.Services.GHService;
using Domain.HikProModels;

namespace PHCAttendanceService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GHAttendaceController : ControllerBase
{
    private readonly GHAttendaceService _service;

    public GHAttendaceController(GHAttendaceService service) => _service = service;

    [HttpGet]
    public Task<TblAttendanceDataReqphcRespDTO> GetGHAttendanceRequests([FromQuery, BindRequired] DateTime date) =>
        _service.GetGHAttendanceRequests(date);

    [HttpPost("hikpro")]
    public Task<GHAttendanceAccessLogRespDTO> GetHikProAccessLogsByCprsAndDate(
        [FromBody] GHAttendanceAccessLogRequestDTO request) =>
        _service.GetHikProAccessLogsByCprsAndDate(request);

    [HttpPost("postlogs")]
    public Task<TblAttendanceDataLogPhcRespDTO> InsertGHAttendanceLogs([FromBody] List<HikProAccessLog> accessLogs) =>
        _service.InsertGHAttendanceLogs(accessLogs);
}
