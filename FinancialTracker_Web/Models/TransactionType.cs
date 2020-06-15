using System.Collections.Generic;

namespace FinancialTracker_Web.Models
{
    public class TransactionType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public bool IsIncome { get; set; }

        public virtual ICollection<Transaction> Transactions { get; set; }

        public TransactionType() {
            Transactions = new HashSet<Transaction>();
        }
    }
}