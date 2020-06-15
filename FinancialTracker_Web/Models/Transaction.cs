using System;
using System.ComponentModel.DataAnnotations;

namespace FinancialTracker_Web.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public int ParentAccountId { get; set; }
        public int TransactionTypeId { get; set; }
        public int? CategoryItemId { get; set; }
        public string OwnerId { get; set; }

        [MaxLength(45)]
        public string Memo { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public DateTime RecordedAt { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }


        public decimal GetAmount() {
            if( this.TransactionType.IsIncome ) { return Amount; } else { return Amount * -1; }
        }

        public virtual BankAccount ParentAccount { get; set; }
        public virtual TransactionType TransactionType { get; set; }
        public virtual CategoryItem CategoryItem { get; set; }
        public virtual ApplicationUser Owner { get; set; }
    }
}