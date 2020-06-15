using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FinancialTracker_Web.Models
{
    public class TransactionType
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(25)]
        public string Name { get; set; }


        [Required]
        [MaxLength(125)]
        public string Description { get; set; }

        [Required]
        public bool IsIncome { get; set; }

        public virtual ICollection<Transaction> Transactions { get; set; }

        public TransactionType() {
            Transactions = new HashSet<Transaction>();
        }
    }
}