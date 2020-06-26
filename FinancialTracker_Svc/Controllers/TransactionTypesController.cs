using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
    [SwaggerControllerOrder(21)]
    public class TransactionTypesController : ApiController
    {
        private const string FRIENDLY_CONTROLLER_NAME = "Types of Transactions";

        private ApiDbContext db = new ApiDbContext();


        [SwaggerOperation(Tags = new[] { FRIENDLY_CONTROLLER_NAME })]
        [Route("TransactionTypes")]
        [HttpGet]
        public async Task<TransactionTypesContainer> TransactionTypes() {
            return await db.GetTransactionTypes();
        }

        [SwaggerOperation(Tags = new[] { FRIENDLY_CONTROLLER_NAME })]
        [Route("TransactionType/{Id}")]
        [HttpGet]
        public async Task<TransactionType> TransactionType(int id) {
            return await db.GetTransactionType(id);
        }

        [SwaggerOperation(Tags = new[] { FRIENDLY_CONTROLLER_NAME })]
        [Route("TransactionType/Create")]
        [HttpPost]
        public async Task<TransactionType> TransactionType_Create(string name, string description, bool isIncome) {
            return await db.CreateTransactionType(name, description, isIncome);
        }

        [SwaggerOperation(Tags = new[] { FRIENDLY_CONTROLLER_NAME })]
        [Route("TransactionType/{Id}/edit")]
        [HttpPatch]
        public async Task<TransactionType> TransactionType_Edit(int id, string newName = null, string newDescription = null, bool? isStillIncome = null) {
            return await db.EditTransactionType(id, newName, newDescription, isStillIncome);
        }

        [SwaggerOperation(Tags = new[] { FRIENDLY_CONTROLLER_NAME })]
        [Route("TransactionType/{Id}/delete")]
        [HttpDelete]
        public async Task<ResultSet> TransactionType_Delete(int id) {
            return await db.DeleteTransactionType(id);
        }
    }
}
