using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.GHOracleModel;

[Table("TBL_ATTENDANCE_DATA_LOG_PHC")]
public class TblAttendanceDataLogPhc
{
    [Column("ID", TypeName = "NUMBER")]
    public decimal Id { get; set; }

    [MaxLength(32), Column("EMP_ID", TypeName = "VARCHAR2(32)")]
    public string? EmpId { get; set; }

    [MaxLength(256), Column("EMP_FIRST_NAME", TypeName = "VARCHAR2(256)")]
    public string? EmpFirstName { get; set; }

    [MaxLength(256), Column("EMP_LAST_NAME", TypeName = "VARCHAR2(256)")]
    public string? EmpLastName { get; set; }

    [MaxLength(32), Column("ACCESS_DATE_TIME", TypeName = "VARCHAR2(32)")]
    public string? AccessDateTime { get; set; }

    [MaxLength(32), Column("ACCESS_DATE", TypeName = "VARCHAR2(32)")]
    public string? AccessDate { get; set; }

    [MaxLength(32), Column("ACCESS_TIME", TypeName = "VARCHAR2(32)")]
    public string? AccessTime { get; set; }

    [MaxLength(128), Column("DEV_NAME", TypeName = "VARCHAR2(128)")]
    public string? DevName { get; set; }

    [MaxLength(128), Column("DEV_SN", TypeName = "VARCHAR2(128)")]
    public string? DevSn { get; set; }
}
