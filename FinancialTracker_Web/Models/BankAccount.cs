﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinancialTracker_Web.Models
{
    public class BankAccount
    {
        public int Id { get; set; }
        public string OwnerId { get; set; }
        public int ParentHouseholdId { get; set; }
        public int AccountTypeId { get; set; }
        public int BankId { get; set; }

        public string Name { get; set; }
        public DateTime Created { get; set; }

        public decimal StartingBalance { get; set; }
        public decimal CurrentBalance { get; set; }

        public decimal? LowBalanceAlertThreshold { get; set; }

        public virtual Household ParentHousehold { get; set; }
        public virtual ApplicationUser Owner { get; set; }
        public virtual BankAccountType AccountType { get; set; }
        public virtual Bank Bank { get; set; }

        public virtual ICollection<Transaction> Transactions { get; set; }


        public BankAccount() {
            Transactions = new HashSet<Transaction>();
        }


    }
}