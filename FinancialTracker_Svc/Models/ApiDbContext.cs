using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Swashbuckle.Swagger.Annotations;

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

        #region error responses

        private HttpResponseException _errNotFound(int? id) {
            return new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound) {
                ReasonPhrase = "Not Found",
                Content = new StringContent(id != null ? $"Unable to locate object with Id={id}" : "Unable to locate objects with specified identities.")
            });
        }
        private HttpResponseException _errNoChange() {
            return new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest) {
                ReasonPhrase = "Bad Request",
                Content = new StringContent("No changes were committed.")
            });
        }

        private HttpResponseException _errDatabaseMsg(SqlException ex) {
            return new HttpResponseException(new HttpResponseMessage(ex.Message.ToLower().Contains("no record") ? HttpStatusCode.NotFound : HttpStatusCode.InternalServerError) {
                ReasonPhrase = "Internal Server Error",
                Content = new StringContent(ex.InnerException != null ? $"A Database Error Occured: {ex.Message} | Inner Error: {ex.Message}" : $"A Database Error Occured: {ex.Message}")
            });
        }
        private HttpResponseException _errGeneralException(Exception ex) {
            return new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError) {
                ReasonPhrase = "Internal Server Error",
                Content = new StringContent(ex.InnerException != null ? $"An Error Occured: {ex.Message} ||| Inner Error: {ex.InnerException.Message}" : $"An Error Occured: {ex.Message}")
            });
        }

        #endregion


        public async Task<HouseholdsContainer> GetHouseholds() {
            return new HouseholdsContainer(await Database.SqlQuery<Household>("exec Household_Fetch").ToListAsync());
        }
        public async Task<Household> GetHousehold(int id) {
            try {
                return await Database.SqlQuery<Household>("exec Household_Fetch @id",
                new SqlParameter("@id", id)).FirstOrDefaultAsync();
            } catch( SqlException sqlex ) {
                throw _errDatabaseMsg(sqlex);

            } catch( HttpResponseException hme ) {
                throw;

            } catch( Exception ex ) {
                throw _errGeneralException(ex);
            }

        }
        public async Task<Household> CreateHousehold(string secret, string name, string greeting) {
            try {
                var houseId = await Database.SqlQuery<int>("exec Household_Create @Name, @Greeting, @Secret",
                                                                 new SqlParameter("@Secret", secret),
                                                                 new SqlParameter("@Name", name ?? ""),
                                                                 new SqlParameter("@Greeting", greeting ?? "")).FirstOrDefaultAsync();
                if( houseId < 1 ) {
                    throw _errNoChange();
                }
                return await GetHousehold(houseId);

            } catch( SqlException sqlex ) {
                throw _errDatabaseMsg(sqlex);

            } catch( HttpResponseException hme ) {
                throw;

            } catch( Exception ex ) {
                throw _errGeneralException(ex);
            }
        }
        public async Task<Household> EditHousehold(string secret, int id, string newName = null, string newGreeting = null) {
            try {
                var rows = await Database.ExecuteSqlCommandAsync("exec Household_Edit @Id, @Secret, @NewName, @NewGreeting",
                                                                    new SqlParameter("@Id", id),
                                                                    new SqlParameter("@Secret", secret),
                                                                    new SqlParameter("@NewName", newName ?? ""),
                                                                    new SqlParameter("@NewGreeting", newGreeting ?? ""));
                if( rows < 0 ) {
                    throw _errNoChange();
                }

                return await GetHousehold(id);

            } catch( SqlException sqlex ) {
                throw _errDatabaseMsg(sqlex);

            } catch( HttpResponseException hme ) {
                throw;

            } catch( Exception ex ) {
                throw _errGeneralException(ex);
            }
        }
        public async Task<ResultSet> DeleteHousehold(string secret, int id) {
            Database.BeginTransaction();
            try {
                var rows = await Database.ExecuteSqlCommandAsync("exec Household_Delete @Id, @Secret",
                                                                 new SqlParameter("@Id", id),
                                                                 new SqlParameter("@Secret", secret));
                if( rows < 1 ) {
                    Database.CurrentTransaction?.Rollback();
                    throw _errNoChange();
                }


                Database.CurrentTransaction.Commit();
                return new ResultSet(false, 0, "Household Deleted Successfully.", id);

            } catch( SqlException sqlex ) {
                Database.CurrentTransaction?.Rollback();
                throw _errDatabaseMsg(sqlex);

            } catch( HttpResponseException hme ) {
                throw;

            } catch( Exception ex ) {
                throw _errGeneralException(ex);
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