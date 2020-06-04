using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Security.Claims;
using System.Security.Policy;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace FinancialTracker_Web.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DisplayName { get; set; }
        public string AvatarPath { get; set; }

        public int? HouseholdId { get; set; }

        public virtual Household Household { get; set; }

        public ICollection<BankAccount> Accounts { get; set; }




        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public System.Data.Entity.DbSet<FinancialTracker_Web.Models.BankAccount> BankAccounts { get; set; }

        public System.Data.Entity.DbSet<FinancialTracker_Web.Models.BankAccountType> BankAccountTypes { get; set; }

        public System.Data.Entity.DbSet<FinancialTracker_Web.Models.Bank> Banks { get; set; }

        public System.Data.Entity.DbSet<FinancialTracker_Web.Models.Household> Households { get; set; }

        public System.Data.Entity.DbSet<FinancialTracker_Web.Models.Category> Categories { get; set; }

        public System.Data.Entity.DbSet<FinancialTracker_Web.Models.CategoryItem> CategoryItems { get; set; }

        public System.Data.Entity.DbSet<FinancialTracker_Web.Models.TransactionType> TransactionTypes { get; set; }

        public System.Data.Entity.DbSet<FinancialTracker_Web.Models.Transaction> Transactions { get; set; }
    }
}