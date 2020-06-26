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
    [SwaggerControllerOrder(0)]
    public class HouseholdsController : ApiController
    {

        private ApiDbContext db = new ApiDbContext();

        [Route("Households")]
        [HttpGet]
        public async Task<HouseholdsContainer> Households() {
            return await db.GetHouseholds();
        }

        [Route("Household/{id}")]
        [HttpGet]
        public async Task<Household> Household(int id) {
            return await db.GetHousehold(id);
        }

        [Route("Household/Create")]
        [HttpPost]
        public async Task<Household> Household_Create(string Name, string Greeting) {
            return await db.CreateHousehold(GetApiKeyFromRequest(Request), Name, Greeting);
        }

        [Route("Household/{id}/edit")]
        [HttpPatch]
        public async Task<Household> Household_Edit(int id, string NewName = null, string NewGreeting = null) {
            return await db.EditHousehold(GetApiKeyFromRequest(Request), id, NewName, NewGreeting);
        }

        [Route("Household/{id}/delete")]
        [HttpDelete]
        public async Task<ResultSet> Household_Delete(int id) {
            return await db.DeleteHousehold(GetApiKeyFromRequest(Request), id);
        }
    }
}
