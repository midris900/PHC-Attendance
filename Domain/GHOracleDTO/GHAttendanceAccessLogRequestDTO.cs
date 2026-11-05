using System.ComponentModel.DataAnnotations;

namespace Domain.GHOracleDTO;

public class GHAttendanceAccessLogRequestDTO
{
    [Required, MinLength(1)]
    public List<string> Cprs { get; set; } = [];

    [Required]
    public DateTime? Date { get; set; }
}
