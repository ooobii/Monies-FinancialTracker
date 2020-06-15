using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace FinancialTracker_Web.Migrations
{
    using FinancialTracker_Web.Models;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<FinancialTracker_Web.Models.AppDbContext>
    {
        public Configuration() {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(FinancialTracker_Web.Models.AppDbContext context) {
            //stores
            RoleManager<IdentityRole> roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var userStore = new UserStore<ApplicationUser>(context);
            var userManager = new UserManager<ApplicationUser>(userStore);

            #region Roles

            if( !context.Roles.Any(r => r.Name == "ServerAdmin") ) {
                roleManager.Create(new IdentityRole { Name = "ServerAdmin" });
            }
            if( !context.Roles.Any(r => r.Name == "Member") ) {
                roleManager.Create(new IdentityRole { Name = "Member" });
            }

            #endregion Roles

            #region Users

            if( !context.Users.Any(u => u.Email == "matt_wendel@hotmail.com") ) {
                var user = new ApplicationUser {
                    UserName = "matt_wendel@hotmail.com",
                    Email = "matt_wendel@hotmail.com",
                    FirstName = "Matthew",
                    LastName = "Wendel",
                    AvatarImagePath = "/Images/avatars/matthew.png",
                    EmailConfirmed = true
                };
                userManager.Create(user, "Abcd3FG#");
                userManager.AddToRole(user.Id, "ServerAdmin");
            }

            #endregion Users

            #region BankAccountTypes

            if( !context.BankAccountTypes.Any(bat => bat.Name == "Checking") ) {
                var acc = new BankAccountType() {
                    Name = "Checking"
                };
                context.BankAccountTypes.Add(acc);
                context.SaveChanges();
            }
            if( !context.BankAccountTypes.Any(bat => bat.Name == "Savings") ) {
                var acc = new BankAccountType() {
                    Name = "Savings"
                };
                context.BankAccountTypes.Add(acc);
                context.SaveChanges();
            }
            if( !context.BankAccountTypes.Any(bat => bat.Name == "Credit Line") ) {
                var acc = new BankAccountType() {
                    Name = "Credit Line"
                };
                context.BankAccountTypes.Add(acc);
                context.SaveChanges();
            }
            if( !context.BankAccountTypes.Any(bat => bat.Name == "Loan") ) {
                var acc = new BankAccountType() {
                    Name = "Loan"
                };
                context.BankAccountTypes.Add(acc);
                context.SaveChanges();
            }

            #endregion BankAccountTypes

            #region TransactionTypes

            //Income
            if( !context.TransactionTypes.Any(tt => tt.Name == "Misc. Income") ) {
                var transType = new TransactionType() {
                    Name = "Misc. Income",
                    Description = "Income that was not planned, or does not have a place within the budget.",
                    IsIncome = true
                };
                context.TransactionTypes.Add(transType);
                context.SaveChanges();
            }
            if( !context.TransactionTypes.Any(tt => tt.Name == "Paycheck") ) {
                var transType = new TransactionType() {
                    Name = "Paycheck",
                    Description = "Income that was earned from employment.",
                    IsIncome = true
                };
                context.TransactionTypes.Add(transType);
                context.SaveChanges();
            }
            if( !context.TransactionTypes.Any(tt => tt.Name == "Refund") ) {
                var transType = new TransactionType() {
                    Name = "Refund",
                    Description = "A purchase that was made previously has been reversed, and the money has been returned.",
                    IsIncome = true
                };
                context.TransactionTypes.Add(transType);
                context.SaveChanges();
            }

            //Expenses
            if( !context.TransactionTypes.Any(tt => tt.Name == "Misc. Expenses") ) {
                var transType = new TransactionType() {
                    Name = "Misc. Expenses",
                    Description = "An expense that was not planned, or does not have a place within the budget.",
                    IsIncome = false
                };
                context.TransactionTypes.Add(transType);
                context.SaveChanges();
            }
            if( !context.TransactionTypes.Any(tt => tt.Name == "Bill") ) {
                var transType = new TransactionType() {
                    Name = "Bill",
                    Description = "A reoccurring expense to provide living space & utilities.",
                    IsIncome = false
                };
                context.TransactionTypes.Add(transType);
                context.SaveChanges();
            }
            if( !context.TransactionTypes.Any(tt => tt.Name == "Purchase") ) {
                var transType = new TransactionType() {
                    Name = "Purchase",
                    Description = "A purchase made at a vendor for a product or one-time service.",
                    IsIncome = false
                };
                context.TransactionTypes.Add(transType);
                context.SaveChanges();
            }
            if( !context.TransactionTypes.Any(tt => tt.Name == "Payment") ) {
                var transType = new TransactionType() {
                    Name = "Payment",
                    Description = "A payment towards a loan or credit line.",
                    IsIncome = false
                };
                context.TransactionTypes.Add(transType);
                context.SaveChanges();
            }

            //Transfers
            if( !context.TransactionTypes.Any(tt => tt.Name == "Transfer From") ) {
                var transType = new TransactionType() {
                    Name = "Transfer From",
                    Description = "Funds were withdrawn from this account, and placed into a new account.",
                    IsIncome = false
                };
                context.TransactionTypes.Add(transType);
                context.SaveChanges();
            }
            if( !context.TransactionTypes.Any(tt => tt.Name == "Transfer To") ) {
                var transType = new TransactionType() {
                    Name = "Transfer To",
                    Description = "Funds were deposited into this account directly from another account.",
                    IsIncome = true
                };
                context.TransactionTypes.Add(transType);
                context.SaveChanges();
            }

            #endregion TransactionTypes
        }
    }
}