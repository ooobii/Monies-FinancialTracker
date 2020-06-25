using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinancialTracker_Svc.Models
{
    public class ResultSet
    {
        public bool Error { get; set; }
        public int ResultCode { get; set; }
        public string Message { get; set; }
        public int? AffectedObjectId { get; set; }

        public ResultSet(bool err, int code, string msg, int? objId = null) {
            Error = err;
            ResultCode = code;
            Message = msg;
            AffectedObjectId = objId;
        }
    }
}