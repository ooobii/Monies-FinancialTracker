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
    [SwaggerControllerOrder(3)]
    public class CategoriesController : ApiController
    {

        private ApiDbContext db = new ApiDbContext();

        [Route("Categories")]
        [HttpGet]
        public async Task<CategoriesContainer> Categories() {
            return await db.GetCategories(GetApiKeyFromRequest(Request));
        }

        [Route("Category/{id}")]
        [HttpGet]
        public async Task<Category> Category(int id) {
            return await db.GetCategory(GetApiKeyFromRequest(Request), id);
        }

        [Route("Category/Create")]
        [HttpPost]
        public async Task<Category> Category_Create(string name, string description) {
            return await db.CreateCategory(GetApiKeyFromRequest(Request), name, description);
        }

        [Route("Category/{id}/edit")]
        [HttpPatch]
        public async Task<Category> Category_Edit(int id, string newName = null, string newDescription = null) {
            return await db.EditCategory(GetApiKeyFromRequest(Request), id, newName, newDescription);
        }

        [Route("Category/{id}/delete")]
        [HttpDelete]
        public async Task<ResultSet> Category_Delete(int id) {
            return await db.DeleteCategory(GetApiKeyFromRequest(Request), id);
        }
    }
}
