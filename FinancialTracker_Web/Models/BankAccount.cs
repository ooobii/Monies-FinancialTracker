using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FinancialTracker_Web.Models
{
    public class BankAccount
    {
        public int Id { get; set; }
        [Required]
        public string OwnerId { get; set; }
        [Required]
        public int ParentHouseholdId { get; set; }
        [Required]
        public int AccountTypeId { get; set; }

        [Required]
        public string AccountName { get; set; }
        [Required]
        public DateTime Created { get; set; }


        [Required]
        public decimal StartingBalance { get; set; }

        [Required]
        public decimal CurrentBalance { get; set; }


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