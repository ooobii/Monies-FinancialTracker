using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinancialTracker_Web.Models
{
    public class EditBankAccountModel
    {
        public int Id { get; set; }
        public string AccountName { get; set; }
        public int AccountTypeId { get; set; }
        public decimal? LowBalanceAlertThreshold { get; set; }
    }
}