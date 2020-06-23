using FinancialTracker_Web.Models;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Antlr.Runtime;
using WebGrease.Css.Extensions;

namespace FinancialTracker_Web.Controllers
{
    public class CategoryItemsController : Controller
    {
        private AppDbContext db = new AppDbContext();


        public ActionResult Details(int? id) {
            if(id == null) { return new HttpStatusCodeResult(HttpStatusCode.BadRequest); }
            var ci = db.CategoryItems.Find(id);
            if(ci == null) { return new HttpStatusCodeResult(HttpStatusCode.NotFound); }

            return View(ci);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ParentCategoryId,Name,Description,AmountBudgeted")] CategoryItem categoryItem, string returnUrl) {
            if( ModelState.IsValid ) {
                db.CategoryItems.Add(categoryItem);
                db.SaveChanges();
            }
            return returnUrl == null ? RedirectToAction("Details", "Households") : RedirectToLocal(returnUrl, RedirectToAction("Details", "Households"));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditCategoryItemModel model, string returnUrl) {
            if( ModelState.IsValid ) {
                var catItem = db.CategoryItems.Find(model.Id);
                if( catItem != null ) {
                    catItem.Name = model.Name;
                    catItem.Description = model.Description;
                    catItem.AmountBudgeted = model.AmountBudgeted;

                    db.SaveChanges();
                }
            }
            return returnUrl == null ? RedirectToAction("Details", "Households") : RedirectToLocal(returnUrl, RedirectToAction("Details", "Households"));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, string returnUrl) {
            var categoryItem = db.CategoryItems.Find(id);
            if(categoryItem == null) { return new HttpStatusCodeResult(HttpStatusCode.BadRequest); }

            var category = db.Categories.Find(categoryItem.ParentCategoryId);
            if( category == null) { return new HttpStatusCodeResult(HttpStatusCode.BadRequest); }

            //remove transactions from category item, and set them to uncategorized.
            var transactions = db.Transactions.Where(t => t.CategoryItemId == categoryItem.Id).ToList();
            foreach(var t in transactions) {
                t.CategoryItemId = null;
            }

            //remove category item from parent category.
            category.CategoryItems.Remove(categoryItem);

            //remove category item from db
            db.CategoryItems.Remove(categoryItem);

            //save and direct to return
            db.SaveChanges();
            return returnUrl == null ? RedirectToAction("Details", "Households") : RedirectToLocal(returnUrl, RedirectToAction("Details", "Households"));
        }






        protected override void Dispose(bool disposing) {
            if( disposing ) {
                db.Dispose();
            }
            base.Dispose(disposing);
        }




        #region Helpers 
        private ActionResult RedirectToLocal(string returnUrl, ActionResult fallback = null) {
            if( Url.IsLocalUrl(returnUrl) ) {
                return Redirect(returnUrl);
            }
            return fallback ?? RedirectToAction("Index", "Home");
        }
        #endregion
    }
}