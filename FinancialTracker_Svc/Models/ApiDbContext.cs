using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
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

        public async Task<HouseholdsContainer> GetHouseholds() {
            return new HouseholdsContainer(await Database.SqlQuery<Household>("exec Household_Fetch").ToListAsync());
        }
        public async Task<Household> GetHousehold(int id) {
            return await Database.SqlQuery<Household>("exec Household_Fetch @id", 
                new SqlParameter("@id", id)).FirstOrDefaultAsync();
        }
        public async Task<ResultSet> EditHousehold(string secret, int id, string newName = null, string newGreeting = null) {
            try {
                var rows = await Database.ExecuteSqlCommandAsync("exec Household_Edit @id, @secret, @newName, @newGreeting",
                                                                 new SqlParameter("@id", id),
                                                                 new SqlParameter("@secret", secret),
                                                                 new SqlParameter("@newName", newName ?? ""),
                                                                 new SqlParameter("@newGreeting", newGreeting ?? ""));
                if( rows > 0 ) {
                    return new ResultSet(false, 0, "Household was modified successfully.", id);
                }
                return new ResultSet(true, 100, "No households were affected by the modifications.", id);

            } catch( SqlException sqlex ) {
                return new ResultSet(true, 800, $"A database error occured while attempting to commit the requested household changes: {sqlex.Message}", id);

            } catch (Exception ex) {
                return new ResultSet(true, 500, "An error occured while attempting to commit the requested household changes.", id);
            }
        }
        public async Task<ResultSet> DeleteHousehold(string secret, int id) {
            try {

                var rows = await Database.ExecuteSqlCommandAsync("exec Household_Delete @CallerId, @id, @newName, @newGreeting",
                                                                 new SqlParameter("@id", id));
                if( rows > 0 ) {
                    return new ResultSet(false, 0, "Household was modified successfully.", id);
                }
                return new ResultSet(true, 100, "No households were affected by the modifications.", id);


            } catch( SqlException sqlex ) {
                return new ResultSet(true, 800, $"A database error occured while attempting to commit the requested household changes: {sqlex.Message}", id);

            } catch( Exception ex ) {
                return new ResultSet(true, 500, "An error occured while attempting to commit the requested household changes.", id);
            }
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