using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace FinancialTracker_Web.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser {
        [Required]
        [MinLength(2)]
        [MaxLength(25)]
        public string FirstName { get; set; }

        [Required]
        [MinLength(2)]
        [MaxLength(25)]
        public string LastName { get; set; }

        public string AvatarImagePath { get; set; }

        public int? HouseholdId { get; set; }

        public virtual Household Household { get; set; }

        public ICollection<BankAccount> Accounts { get; set; }


        public string GetShortName() {
            return this.LastName.Length > 0 ? $"{this.FirstName} {this.LastName.Substring(0, 1)}" : $"{this.FirstName}";
        }
        public string GetFullName() {
            return this.FirstName + " " + this.LastName;
        }

        public static ApplicationUser GetFromDb(IPrincipal user, AppDbContext context = null) {
            if( context == null ) { context = new AppDbContext(); }

            return context.Users.Find(user.Identity.GetUserId());
        }
        public static ApplicationUser GetFromDb(string id, AppDbContext context = null) {
            if( context == null ) { context = new AppDbContext(); }

            return context.Users.Find(id);
        }

        public static Household GetParentHousehold(IPrincipal user, AppDbContext context = null) {
            if( context == null ) { context = new AppDbContext(); }

            return context.Users.Find(user.Identity.GetUserId()).Household;
        }
        public static Household GetParentHousehold(string id, AppDbContext context = null) {
            if( context == null ) { context = new AppDbContext(); }

            return context.Users.Find(id).Household;
        }

        public static int? GetParentHouseholdId(IPrincipal user, AppDbContext context = null) {
            if( context == null ) { context = new AppDbContext(); }

            return context.Users.Find(user.Identity.GetUserId()).HouseholdId;
        }
        public static int? GetParentHouseholdId(string id, AppDbContext context = null) {
            if( context == null ) { context = new AppDbContext(); }

            return context.Users.Find(id).HouseholdId;
        }


        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager) {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext()
            : base("DefaultConnection", throwIfV1Schema: false) {
        }

        public static AppDbContext Create() {
            return new AppDbContext();
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