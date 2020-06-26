using System;
using System.Collections.Generic;

namespace FinancialTracker_Svc.Models
{
    public class BankAccount
    {
        public int Id { get; set; }
        public string OwnerId { get; set; }
        public int ParentHouseholdId { get; set; }
        public int AccountTypeId { get; set; }
        public string AccountName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public decimal StartingBalance { get; set; }
        public decimal? LowBalanceAlertThreshold { get; set; }

    }

    public class BankAccountsContainer
    {
        public ICollection<BankAccount> BankAccounts { get; set; }

        public BankAccountsContainer(ICollection<BankAccount> accs) {
            BankAccounts = accs;
        }
    }
}