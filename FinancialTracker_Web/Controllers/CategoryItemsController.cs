using FinancialTracker_Web.Models;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Antlr.Runtime;

namespace FinancialTracker_Web.Controllers
{
    public class CategoryItemsController : Controller
    {
        private AppDbContext db = new AppDbContext();




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
            CategoryItem categoryItem = db.CategoryItems.Find(id);
            
            db.CategoryItems.Remove(categoryItem);
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