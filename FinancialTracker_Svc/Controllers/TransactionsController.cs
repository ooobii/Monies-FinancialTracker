using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using FinancialTracker_Svc.Models;
using Newtonsoft.Json;
using Swashbuckle.Swagger.Annotations;
using static FinancialTracker_Svc.Helpers.Util;

namespace FinancialTracker_Svc.Controllers
{
    [SwaggerControllerOrder(2)]
    public class TransactionsController : ApiController
    {

        private ApiDbContext db = new ApiDbContext();


        [Route("Transactions")]
        [HttpGet]
        public async Task<TransactionsContainer> Transactions() {
            return await db.GetTransactions(GetApiKeyFromRequest(Request));
        }

        [Route("Transaction/{Id}")]
        [HttpGet]
        public async Task<Transaction> Transaction(int id) {
            return await db.GetTransaction(GetApiKeyFromRequest(Request), id);
        }

        [Route("Transaction/Create")]
        [HttpPost]
        public async Task<Transaction> Transaction_Create(string name, string memo, decimal amount, string occuredAt, int parentAccountId, int transactionTypeId, int? subCategoryId = null) {
            return await db.CreateTransaction(GetApiKeyFromRequest(Request), name, memo, amount, occuredAt, parentAccountId, transactionTypeId, subCategoryId);
        }

        [Route("Transaction/{Id}/edit")]
        [HttpPatch]
        public async Task<Transaction> Transaction_Edit(int id, string newName = null, string newMemo = null, decimal? newAmount = null, string newOccuredAt = null, int? newParentAccountId = null, int? newTransactionTypeId = null, int? newSubCategoryId = null) {
            return await db.EditTransaction(GetApiKeyFromRequest(Request), id, newName, newMemo, newAmount, newOccuredAt, newParentAccountId, newTransactionTypeId, newSubCategoryId);
        }

        [Route("Transaction/{Id}/delete")]
        [HttpDelete]
        public async Task<ResultSet> Transaction_Delete(int id) {
            return await db.DeleteTransaction(GetApiKeyFromRequest(Request), id);
        }
    }
}
