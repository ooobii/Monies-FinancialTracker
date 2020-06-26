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
using static FinancialTracker_Svc.Helpers.Util;

namespace FinancialTracker_Svc.Controllers
{
    public class BankAccountTypesController : ApiController
    {
        private ApiDbContext db = new ApiDbContext();


        [Route("BankAccountTypes")]
        [HttpGet]
        public async Task<BankAccountTypesContainer> BankAccountTypes() {
            return await db.GetBankAccountTypes();
        }

        [Route("BankAccountType/{Id}")]
        [HttpGet]
        public async Task<BankAccountType> BankAccountType(int id) {
            return await db.GetBankAccountType(id);
        }

        [Route("BankAccountType/Create")]
        [HttpPost]
        public async Task<BankAccountType> BankAccountType_Create(string name) {
            return await db.CreateBankAccountType(name);
        }

        [Route("BankAccountType/{Id}/edit")]
        [HttpPatch]
        public async Task<BankAccountType> BankAccountType_Edit(int id, string newname = null) {
            return await db.EditBankAccountType(id, newname);
        }

        [Route("BankAccountType/{Id}/delete")]
        [HttpDelete]
        public async Task<ResultSet> BankAccountType_Delete(int id) {
            return await db.DeleteBankAccountType(id);
        }
    }
}
