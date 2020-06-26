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
    [SwaggerControllerOrder(4)]
    public class CategoryItemsController : ApiController
    {

        private const string FRIENDLY_CONTROLLER_NAME = "Subcategories";

        private ApiDbContext db = new ApiDbContext();


        [SwaggerOperation(Tags = new[] { FRIENDLY_CONTROLLER_NAME })]
        [Route("CategoryItems")]
        [HttpGet]
        public async Task<CategoryItemsContainer> CategoryItems(int? id = null, int? categoryId = null) {
            return await db.GetCategoryItems(GetApiKeyFromRequest(Request), id, categoryId);
        }

        [SwaggerOperation(Tags = new[] { FRIENDLY_CONTROLLER_NAME })]
        [Route("CategoryItem/Create")]
        [HttpPost]
        public async Task<CategoryItemsContainer> CategoryItem_Create(string name, string description, decimal monthBudget, int parentCategoryId) {
            return await db.CreateCategoryItem(GetApiKeyFromRequest(Request), name, description, monthBudget, parentCategoryId);
        }

        [SwaggerOperation(Tags = new[] { FRIENDLY_CONTROLLER_NAME })]
        [Route("CategoryItem/{id}/edit")]
        [HttpPatch]
        public async Task<CategoryItemsContainer> CategoryItem_Edit(int id, string newName = null, string newDescription = null, decimal? newMonthBudget = null, int? newParentCategoryId = null) {
            return await db.EditCategoryItem(GetApiKeyFromRequest(Request), id, newName, newDescription, newMonthBudget, newParentCategoryId);
        }

        [SwaggerOperation(Tags = new[] { FRIENDLY_CONTROLLER_NAME })]
        [Route("CategoryItem/{id}/delete")]
        [HttpDelete]
        public async Task<ResultSet> CategoryItem_Delete(int id) {
            return await db.DeleteCategoryItem(GetApiKeyFromRequest(Request), id);
        }
    }
}
