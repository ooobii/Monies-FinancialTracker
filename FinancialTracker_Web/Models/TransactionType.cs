using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinancialTracker_Web.Models
{
    public class TransactionType
    {
        public int Id { get; set; }
        public string Name { get; set; }


        public virtual ICollection<Transaction> Transactions { get; set; }



        public TransactionType() {
            Transactions = new HashSet<Transaction>();
        }
    }
}