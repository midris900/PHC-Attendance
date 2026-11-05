using Domain.HikProModels;

namespace Domain.GHOracleDTO;

public class GHAttendanceAccessLogRespDTO
{
    public string? RESPONSE_CODE { get; set; }
    public string? RESPONSE_DESCRPTION { get; set; }
    public List<HikProAccessLog>? lstData { get; set; }
}
