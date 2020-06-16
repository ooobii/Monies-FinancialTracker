using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace FinancialTracker_Web.Models
{
    public class BankAccount
    {
        public int Id { get; set; }
        public string OwnerId { get; set; }
        public int ParentHouseholdId { get; set; }
        public int AccountTypeId { get; set; }

        [Required]
        public string AccountName { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }
        
        public DateTime? ModifiedAt { get; set; }


        [Required]
        public decimal StartingBalance { get; set; }

        public decimal GetCurrentBalance() {
            if(this.Transactions.Count > 0) {
                return this.StartingBalance + this.Transactions.Sum(t => t.GetAmount());
            } else {
                return this.StartingBalance;
            }
        }


        public decimal? LowBalanceAlertThreshold { get; set; }

        public virtual Household ParentHousehold { get; set; }
        public virtual ApplicationUser Owner { get; set; }
        public virtual BankAccountType AccountType { get; set; }

        public virtual ICollection<Transaction> Transactions { get; set; }

        public BankAccount() {
            Transactions = new HashSet<Transaction>();
        }
    }
}