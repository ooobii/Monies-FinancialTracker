using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace FinancialTracker_Svc.Models
{
    public class ApiDbContext : DbContext
    {
        public ApiDbContext()
            : base("ApiConnection") {
        }

        public static ApiDbContext Create() {
            return new ApiDbContext();
        }

        public System.Data.Entity.DbSet<BankAccount> BankAccounts { get; set; }
        public System.Data.Entity.DbSet<BankAccountType> BankAccountTypes { get; set; }
        public System.Data.Entity.DbSet<Household> Households { get; set; }
        public System.Data.Entity.DbSet<Category> Categories { get; set; }
        public System.Data.Entity.DbSet<CategoryItem> CategoryItems { get; set; }
        public System.Data.Entity.DbSet<TransactionType> TransactionTypes { get; set; }
        public System.Data.Entity.DbSet<Transaction> Transactions { get; set; }
        public System.Data.Entity.DbSet<Invitation> Invitations { get; set; }
    }
}