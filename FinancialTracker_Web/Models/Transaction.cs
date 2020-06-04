using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinancialTracker_Web.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public int ParentAccountId { get; set; }
        public int TransactionTypeId { get; set; }
        public int? CategoryItemId { get; set; }
        public string OwnerId { get; set; }

        public string Memo { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedAt { get; set; }


        public virtual BankAccount ParentAccount { get; set; }
        public virtual TransactionType TransactionType { get; set; }
        public virtual CategoryItem CategoryItem { get; set; }
        public virtual ApplicationUser Owner { get; set; }

    }
}