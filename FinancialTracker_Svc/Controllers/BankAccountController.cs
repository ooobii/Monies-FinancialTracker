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
    [SwaggerControllerOrder(1)]
    public class BankAccountController : ApiController
    {
        private const string FRIENDLY_CONTROLLER_NAME = "Bank Accounts";

        private ApiDbContext db = new ApiDbContext();


        [SwaggerOperation(Tags = new[] { FRIENDLY_CONTROLLER_NAME })]
        [Route("BankAccounts")]
        [HttpGet]
        public async Task<BankAccountsContainer> BankAccounts() {
            return await db.GetBankAccounts(GetApiKeyFromRequest(Request));
        }

        [SwaggerOperation(Tags = new[] { FRIENDLY_CONTROLLER_NAME })]
        [Route("BankAccount/{Id}")]
        [HttpGet]
        public async Task<BankAccount> BankAccount(int id) {
            return await db.GetBankAccount(GetApiKeyFromRequest(Request), id);
        }

        [SwaggerOperation(Tags = new[] { FRIENDLY_CONTROLLER_NAME })]
        [Route("BankAccount/Create")]
        [HttpPost]
        public async Task<BankAccount> BankAccount_Create(string name, int typeId, decimal startBalance, decimal? lowBalanceAlert) {
            return await db.CreateBankAccount(GetApiKeyFromRequest(Request), name, typeId, startBalance, lowBalanceAlert);
        }

        [SwaggerOperation(Tags = new[] { FRIENDLY_CONTROLLER_NAME })]
        [Route("BankAccount/{Id}/edit")]
        [HttpPatch]
        public async Task<BankAccount> BankAccount_Edit(int id, string newName = null, int? newType = null, decimal? newLowBalanceAlert = null) {
            return await db.EditBankAccount(GetApiKeyFromRequest(Request), id, newName, newType, newLowBalanceAlert);
        }

        [SwaggerOperation(Tags = new[] { FRIENDLY_CONTROLLER_NAME })]
        [Route("BankAccount/{Id}/delete")]
        [HttpDelete]
        public async Task<ResultSet> BankAccount_Delete(int id) {
            return await db.DeleteBankAccount(GetApiKeyFromRequest(Request), id);
        }
    }
}
