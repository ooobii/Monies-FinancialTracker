using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;

namespace FinancialTracker_Web.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public int ParentAccountId { get; set; }
        public int TransactionTypeId { get; set; }
        public int? CategoryItemId { get; set; }
        public string OwnerId { get; set; }

        [Required]
        [MaxLength(45)]
        public string Name { get; set; }

        [MaxLength(120)]
        public string Memo { get; set; }

        [Required]
        [Range(0.0, int.MaxValue)]
        public decimal Amount { get; set; }

        [Required]
        public DateTime OccuredAt { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }



        public decimal GetAmount() {
            if( this.TransactionType.IsIncome ) { return Amount; } else { return Amount * -1; }
        }

        public decimal GetPercentOfBudget() {
            if( CategoryItem.AmountBudgeted != null ) return Amount / CategoryItem.AmountBudgeted.Value;
            return 0;
        }

        public virtual BankAccount ParentAccount { get; set; }
        public virtual TransactionType TransactionType { get; set; }
        public virtual CategoryItem CategoryItem { get; set; }
        public virtual ApplicationUser Owner { get; set; }


    }
}