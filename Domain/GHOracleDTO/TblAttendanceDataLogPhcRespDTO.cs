using Domain.CSBImportModels;
using Domain.GHOracleModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.GHOracleDTO
{
    public class TblAttendanceDataLogPhcRespDTO
    {
        public string RESPONSE_CODE { get; set; }
        public string RESPONSE_DESCRPTION { get; set; }
        public List<TblAttendanceDataLogPhc>? lstData { get; set; }
    }
}
