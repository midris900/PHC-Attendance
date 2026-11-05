using Domain.GHOracleDTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using PHCAttendanceService.Services;

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
}
