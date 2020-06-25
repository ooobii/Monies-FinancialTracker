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

namespace FinancialTracker_Svc.Controllers
{
    public class HouseholdsController : ApiController
    {
        private ApiDbContext db = new ApiDbContext();


        [Route("GetHouseholds")]
        [HttpGet]
        public async Task<HouseholdsContainer> Households() {
            return await db.GetHouseholds();
        }

        [Route("Household/{id}")]
        [HttpGet]
        public async Task<Household> Household(int HouseholdId) {
            return await db.GetHousehold(HouseholdId);
        }

        [Route("Household/{id}/edit")]
        [HttpPatch]
        public async Task<ResultSet> Household_Edit(string secret, int HouseholdId, string NewName = null, string NewGreeting = null) {
            return await db.EditHousehold(secret, HouseholdId, NewName, NewGreeting);
        }

        [Route("Household/{id}/delete")]
        [HttpDelete]
        public async Task<ResultSet> Household_Delete(string secret, int HouseholdId) {
            return await db.DeleteHousehold(HouseholdId);
        }

    }
}
