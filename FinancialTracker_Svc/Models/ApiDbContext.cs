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

        //Household Methods(0)
        public async Task<HouseholdsContainer> GetHouseholds() {
            return new HouseholdsContainer(await Database.SqlQuery<Household>("exec Household_Fetch").ToListAsync());
        }
        public async Task<Household> GetHousehold(int id) {
            try {
                return await Database.SqlQuery<Household>("exec Household_Fetch @id",
                new SqlParameter("@id", id)).FirstOrDefaultAsync();
            } catch( SqlException sqlex ) {
                throw _errDatabaseMsg(sqlex);

            } catch( Exception ex ) {
                throw _errGeneralException(ex);
            }

        }
        public async Task<Household> CreateHousehold(string secret, string name, string greeting) {
            try {
                var houseId = await Database.SqlQuery<int>("exec Household_Create @Secret, @Name, @Greeting",
                                                                 new SqlParameter("@Secret", secret),
                                                                 new SqlParameter("@Name", name ?? ""),
                                                                 new SqlParameter("@Greeting", greeting ?? "")).FirstOrDefaultAsync();
                if( houseId < 1 ) {
                    throw _errNoChange();
                }
                return await GetHousehold(houseId);

            } catch( SqlException sqlex ) {
                throw _errDatabaseMsg(sqlex);

            } catch( HttpResponseException ) {
                throw;

            } catch( Exception ex ) {
                throw _errGeneralException(ex);
            }
        }
        public async Task<Household> EditHousehold(string secret, int id, string newName = null, string newGreeting = null) {
            try {
                var rows = await Database.ExecuteSqlCommandAsync("exec Household_Edit @Secret, @Id, @NewName, @NewGreeting",
                                                                    new SqlParameter("@Id", id),
                                                                    new SqlParameter("@Secret", secret),
                                                                    new SqlParameter("@NewName", (object)newName ?? DBNull.Value),
                                                                    new SqlParameter("@NewGreeting", (object)newGreeting ?? DBNull.Value));
                if( rows < 0 ) {
                    throw _errNoChange();
                }

                return await GetHousehold(id);

            } catch( SqlException sqlex ) {
                throw _errDatabaseMsg(sqlex);

            } catch( HttpResponseException ) {
                throw;

            } catch( Exception ex ) {
                throw _errGeneralException(ex);
            }
        }
        public async Task<ResultSet> DeleteHousehold(string secret, int id) {
            Database.BeginTransaction();
            try {
                var rows = await Database.ExecuteSqlCommandAsync("exec Household_Delete @Secret, @Id",
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

            } catch( HttpResponseException ) {
                throw;

            } catch( Exception ex ) {
                throw _errGeneralException(ex);
            }
        }


        //Bank Account Methods(1)
        public async Task<BankAccountsContainer> GetBankAccounts(string secret) {
            return new BankAccountsContainer(await Database.SqlQuery<BankAccount>("exec BankAccount_Fetch @Secret", new SqlParameter("@Secret", secret)).ToListAsync());
        }
        public async Task<BankAccount> GetBankAccount(string secret, int id) {
            try {
                return await Database.SqlQuery<BankAccount>("exec BankAccount_Fetch @Secret, @id",
                    new SqlParameter("@Secret", secret),
                    new SqlParameter("@id", id)).FirstOrDefaultAsync();
            } catch( SqlException sqlex ) {
                throw _errDatabaseMsg(sqlex);

            } catch( Exception ex ) {
                throw _errGeneralException(ex);
            }

        }
        public async Task<BankAccount> CreateBankAccount(string secret, string name, int type, decimal startBalance, decimal? lowBalanceAlert) {
            try {
                var accId = await Database.SqlQuery<int>("exec BankAccount_Create @Secret, @Name, @Type, @StartingBalance, @LowBalanceAlert",
                                                                 new SqlParameter("@Secret", secret),
                                                                 new SqlParameter("@Name", name),
                                                                 new SqlParameter("@Type", type),
                                                                 new SqlParameter("@StartingBalance", startBalance),
                                                                 new SqlParameter("@LowBalanceAlert", lowBalanceAlert)).FirstOrDefaultAsync();
                if( accId < 1 ) {
                    throw _errNoChange();
                }
                return await GetBankAccount(secret, accId);

            } catch( SqlException sqlex ) {
                throw _errDatabaseMsg(sqlex);

            } catch( HttpResponseException ) {
                throw;

            } catch( Exception ex ) {
                throw _errGeneralException(ex);
            }
        }
        public async Task<BankAccount> EditBankAccount(string secret, int id, string newName = null, int? newType = null, decimal? newLowBalanceAlert = null) {
            try {

                var rows = await Database.ExecuteSqlCommandAsync("exec BankAccount_Edit @Secret, @Id, @NewName, @NewType, @NewLowBalanceAlert",
                                                                 new SqlParameter("@Secret", secret),
                                                                 new SqlParameter("@Id", id),
                                                                 new SqlParameter("@NewName", (object)newName ?? DBNull.Value),
                                                                 new SqlParameter("@NewType", (object)newType ?? DBNull.Value),
                                                                 new SqlParameter("@NewLowBalanceAlert", (object)newLowBalanceAlert ?? DBNull.Value));
                if( rows < 0 ) {
                    throw _errNoChange();
                }

                return await GetBankAccount(secret, id);

            } catch( SqlException sqlex ) {
                throw _errDatabaseMsg(sqlex);

            } catch( HttpResponseException ) {
                throw;

            } catch( Exception ex ) {
                throw _errGeneralException(ex);
            }
        }
        public async Task<ResultSet> DeleteBankAccount(string secret, int id) {
            Database.BeginTransaction();
            try {
                var rows = await Database.ExecuteSqlCommandAsync("exec BankAccount_Delete @Secret, @Id",
                                                                 new SqlParameter("@Id", id),
                                                                 new SqlParameter("@Secret", secret));
                if( rows < 1 ) {
                    Database.CurrentTransaction?.Rollback();
                    throw _errNoChange();
                }


                Database.CurrentTransaction.Commit();
                return new ResultSet(false, 0, "Bank Account Deleted Successfully.", id);

            } catch( SqlException sqlex ) {
                Database.CurrentTransaction?.Rollback();
                throw _errDatabaseMsg(sqlex);

            } catch( HttpResponseException ) {
                throw;

            } catch( Exception ex ) {
                throw _errGeneralException(ex);
            }
        }


        //Transactions(2)
        public async Task<TransactionsContainer> GetTransactions(string secret) {
            return new TransactionsContainer(await Database.SqlQuery<Transaction>("exec Transaction_Fetch @Secret", new SqlParameter("@Secret", secret)).ToListAsync());
        }
        public async Task<Transaction> GetTransaction(string secret, int id) {
            try {
                return await Database.SqlQuery<Transaction>("exec Transaction_Fetch @Secret, @id",
                    new SqlParameter("@Secret", secret),
                    new SqlParameter("@id", id)).FirstOrDefaultAsync();
            } catch( SqlException sqlex ) {
                throw _errDatabaseMsg(sqlex);

            } catch( Exception ex ) {
                throw _errGeneralException(ex);
            }

        }
        public async Task<Transaction> CreateTransaction(string secret, string name, string memo, decimal amount, string occuredAt, int parentAccountId, int transactionTypeId, int? subCategoryId = null) {
            try {
                var transId = await Database.SqlQuery<int>("exec Transaction_Create @Secret, @Name, @Memo, @Amount, @OccuredAt, @ParentAccountId, @TransactionTypeId, @CategoryItemId",
                                                                 new SqlParameter("@Secret", secret),
                                                                 new SqlParameter("@Name", name),
                                                                 new SqlParameter("@Memo", memo),
                                                                 new SqlParameter("@Amount", amount.ToString("0.00")),
                                                                 new SqlParameter("@OccuredAt", DateTime.Parse(occuredAt)),
                                                                 new SqlParameter("@ParentAccountId", parentAccountId),
                                                                 new SqlParameter("@TransactionTypeId", transactionTypeId),
                                                                 new SqlParameter("@CategoryItemId", subCategoryId != null ? (object)subCategoryId.Value : DBNull.Value)).FirstOrDefaultAsync();
                if( transId < 1 ) {
                    throw _errNoChange();
                }
                return await GetTransaction(secret, transId);

            } catch( SqlException sqlex ) {
                throw _errDatabaseMsg(sqlex);

            } catch( HttpResponseException ) {
                throw;

            } catch( Exception ex ) {
                throw _errGeneralException(ex);
            }
        }
        public async Task<Transaction> EditTransaction(string secret, int id, string name = null, string memo = null, decimal? amount = null, string occuredAt = null, int? parentAccountId = null, int? transactionTypeIdk = null, int? subCategoryId = null) {
            try {

                var rows = await Database.ExecuteSqlCommandAsync("exec Transaction_Edit @Secret, @Id, @NewName, @NewMemo, @NewAmount, @NewOccuredAt, @NewParentAccountId, @NewTransactionTypeId, @NewCategoryItemId",
                                                                 new SqlParameter("@Secret", secret),
                                                                 new SqlParameter("@Id", id),
                                                                 new SqlParameter("@NewName", (object)name ?? DBNull.Value),
                                                                 new SqlParameter("@NewMemo", (object)memo ?? DBNull.Value),
                                                                 new SqlParameter("@NewAmount", amount != null ? (object)(amount.Value.ToString("0.00")) : DBNull.Value),
                                                                 new SqlParameter("@NewOccuredAt", occuredAt != null ? (object)( DateTime.Parse(occuredAt) ) : DBNull.Value),
                                                                 new SqlParameter("@NewParentAccountId", (object)parentAccountId ?? DBNull.Value),
                                                                 new SqlParameter("@NewTransactionTypeId", (object)transactionTypeIdk ?? DBNull.Value),
                                                                 new SqlParameter("@NewCategoryItemId", (object)subCategoryId ?? DBNull.Value));
                if( rows < 0 ) {
                    throw _errNoChange();
                }

                return await GetTransaction(secret, id);

            } catch( SqlException sqlex ) {
                throw _errDatabaseMsg(sqlex);

            } catch( HttpResponseException ) {
                throw;

            } catch( Exception ex ) {
                throw _errGeneralException(ex);
            }
        }
        public async Task<ResultSet> DeleteTransaction(string secret, int id) {
            Database.BeginTransaction();
            try {
                var rows = await Database.ExecuteSqlCommandAsync("exec Transaction_Delete @Secret, @Id",
                                                                 new SqlParameter("@Id", id),
                                                                 new SqlParameter("@Secret", secret));
                if( rows < 1 ) {
                    Database.CurrentTransaction?.Rollback();
                    throw _errNoChange();
                }


                Database.CurrentTransaction.Commit();
                return new ResultSet(false, 0, "Transaction Deleted Successfully.", id);

            } catch( SqlException sqlex ) {
                Database.CurrentTransaction?.Rollback();
                throw _errDatabaseMsg(sqlex);

            } catch( HttpResponseException ) {
                throw;

            } catch( Exception ex ) {
                throw _errGeneralException(ex);
            }
        }





        //BankAccountTypeMethods(20)
        public async Task<BankAccountTypesContainer> GetBankAccountTypes() {
            return new BankAccountTypesContainer(await Database.SqlQuery<BankAccountType>("exec BankAccountType_Fetch").ToListAsync());
        }
        public async Task<BankAccountType> GetBankAccountType(int id) {
            try {
                return await Database.SqlQuery<BankAccountType>("exec BankAccountType_Fetch @id",
                new SqlParameter("@id", id)).FirstOrDefaultAsync();
            } catch( SqlException sqlex ) {
                throw _errDatabaseMsg(sqlex);

            } catch( HttpResponseException ) {
                throw;

            } catch( Exception ex ) {
                throw _errGeneralException(ex);
            }

        }
        public async Task<BankAccountType> CreateBankAccountType(string name) {
            try {
                var batId = await Database.SqlQuery<int>("exec BankAccountType_Create @Name",
                                                                 new SqlParameter("@Name", name ?? "")).FirstOrDefaultAsync();
                if( batId < 1 ) {
                    throw _errNoChange();
                }
                return await GetBankAccountType(batId);

            } catch( SqlException sqlex ) {
                throw _errDatabaseMsg(sqlex);

            } catch( HttpResponseException ) {
                throw;

            } catch( Exception ex ) {
                throw _errGeneralException(ex);
            }
        }
        public async Task<BankAccountType> EditBankAccountType(int id, string newName = null) {
            try {
                var rows = await Database.ExecuteSqlCommandAsync("exec BankAccountType_Edit @Id, @NewName",
                                                                    new SqlParameter("@Id", id),
                                                                    new SqlParameter("@NewName", (object)newName ?? DBNull.Value));
                if( rows < 1 ) {
                    throw _errNoChange();
                }

                return await GetBankAccountType(id);

            } catch( SqlException sqlex ) {
                throw _errDatabaseMsg(sqlex);

            } catch( HttpResponseException ) {
                throw;

            } catch( Exception ex ) {
                throw _errGeneralException(ex);
            }
        }
        public async Task<ResultSet> DeleteBankAccountType(int id) {
            Database.BeginTransaction();
            try {
                var rows = await Database.ExecuteSqlCommandAsync("exec BankAccountType_Delete @Id",
                                                                 new SqlParameter("@Id", id));
                if( rows < 1 ) {
                    Database.CurrentTransaction?.Rollback();
                    throw _errNoChange();
                }


                Database.CurrentTransaction.Commit();
                return new ResultSet(false, 0, "BankAccountType Deleted Successfully.", id);

            } catch( SqlException sqlex ) {
                Database.CurrentTransaction?.Rollback();
                throw _errDatabaseMsg(sqlex);

            } catch( HttpResponseException ) {
                throw;

            } catch( Exception ex ) {
                throw _errGeneralException(ex);
            }
        }


        //TransactionTypeMethods(21)
        public async Task<TransactionTypesContainer> GetTransactionTypes() {
            return new TransactionTypesContainer(await Database.SqlQuery<TransactionType>("exec TransactionType_Fetch").ToListAsync());
        }
        public async Task<TransactionType> GetTransactionType(int id) {
            try {
                return await Database.SqlQuery<TransactionType>("exec TransactionType_Fetch @id",
                new SqlParameter("@id", id)).FirstOrDefaultAsync();
            } catch( SqlException sqlex ) {
                throw _errDatabaseMsg(sqlex);

            } catch( Exception ex ) {
                throw _errGeneralException(ex);
            }

        }
        public async Task<TransactionType> CreateTransactionType(string name, string description, bool isincome) {
            try {
                var ttId = await Database.SqlQuery<int>("exec TransactionType_Create @Name, @Description, @IsIncome",
                                                                 new SqlParameter("@Name", name ?? ""),
                                                                 new SqlParameter("@Description", description),
                                                                 new SqlParameter("@IsIncome", isincome)).FirstOrDefaultAsync();
                if( ttId < 1 ) {
                    throw _errNoChange();
                }
                return await GetTransactionType(ttId);

            } catch( SqlException sqlex ) {
                throw _errDatabaseMsg(sqlex);

            } catch( HttpResponseException ) {
                throw;

            } catch( Exception ex ) {
                throw _errGeneralException(ex);
            }
        }
        public async Task<TransactionType> EditTransactionType(int id, string newName = null, string newDescription = null, bool? isStillIncome = null) {
            try {
                var rows = await Database.ExecuteSqlCommandAsync("exec TransactionType_Edit @Id, @NewName, @NewDescription, @IsStillIncome",
                                                                    new SqlParameter("@Id", id),
                                                                    new SqlParameter("@NewName", (object)newName ?? DBNull.Value),
                                                                    new SqlParameter("@Description", (object)newDescription ?? DBNull.Value),
                                                                    new SqlParameter("@IsIncome", (object)isStillIncome ?? DBNull.Value));
                if( rows < 1 ) {
                    throw _errNoChange();
                }

                return await GetTransactionType(id);

            } catch( SqlException sqlex ) {
                throw _errDatabaseMsg(sqlex);

            } catch( HttpResponseException ) {
                throw;

            } catch( Exception ex ) {
                throw _errGeneralException(ex);
            }
        }
        public async Task<ResultSet> DeleteTransactionType(int id) {
            Database.BeginTransaction();
            try {
                var rows = await Database.ExecuteSqlCommandAsync("exec TransactionType_Delete @Id",
                                                                 new SqlParameter("@Id", id));
                if( rows < 1 ) {
                    Database.CurrentTransaction?.Rollback();
                    throw _errNoChange();
                }


                Database.CurrentTransaction.Commit();
                return new ResultSet(false, 0, "Transaction Type Deleted Successfully.", id);

            } catch( SqlException sqlex ) {
                Database.CurrentTransaction?.Rollback();
                throw _errDatabaseMsg(sqlex);

            } catch( HttpResponseException ) {
                throw;

            } catch( Exception ex ) {
                throw _errGeneralException(ex);
            }
        }

    }
}