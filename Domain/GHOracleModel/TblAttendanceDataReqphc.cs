using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.GHOracleModel;

[Table("TBL_ATTENDANCE_DATA_REQPHC")]
public class TblAttendanceDataReqphc
{
    [MaxLength(30), Column("TADR_CPR", TypeName = "VARCHAR2(30)")]
    public string? TadrCpr { get; set; }

    [Column("TADR_INSERT_DATE", TypeName = "DATE")]
    public DateTime? TadrInsertDate { get; set; }

    [MaxLength(30), Column("TADR_INSERT_BY", TypeName = "VARCHAR2(30)")]
    public string? TadrInsertBy { get; set; }

    [Column("TADR_FROM_DATE", TypeName = "DATE")]
    public DateTime? TadrFromDate { get; set; }

    [Column("TADR_TO_DATE", TypeName = "DATE")]
    public DateTime? TadrToDate { get; set; }
}
