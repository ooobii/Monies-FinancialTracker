using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinancialTracker_Web.Models
{
    public class EditTransactionModel
    {
        public int Id { get; set; }
        public int TransactionTypeId { get; set; }
        public int? CategoryItemId { get; set; }
        public string Name { get; set; }
        public string Memo { get; set; }
        public decimal Amount { get; set; }
        public DateTime OccuredAt { get; set; }
    }
}