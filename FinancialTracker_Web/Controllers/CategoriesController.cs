using System;
using FinancialTracker_Web.Models;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace FinancialTracker_Web.Controllers
{
    public class CategoriesController : Controller
    {
        private AppDbContext db = new AppDbContext();

        public ActionResult Index() {
            var categories = db.Categories.Include(c => c.ParentHousehold);
            return View(categories.ToList());
        }
        public ActionResult Details(int? id) {
            if( id == null ) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Find(id);
            if( category == null || category.ParentHouseholdId != ApplicationUser.GetParentHouseholdId(User, db) ) {
                return HttpNotFound();
            }
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ParentHouseholdId,Name,Description")] Category category, string returnUrl) {
            if( ModelState.IsValid ) {
                category.CreatedAt = DateTime.Now;
                db.Categories.Add(category);
                db.SaveChanges();
                return returnUrl == null ? RedirectToAction("Details", "Households") : RedirectToLocal(returnUrl, RedirectToAction("Details", "Households"));
            }
            return returnUrl == null ? RedirectToAction("Details", "Households") : RedirectToLocal(returnUrl, RedirectToAction("Details", "Households"));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditCategoryModel model, string returnUrl) {
            if( ModelState.IsValid ) {
                var cat = db.Categories.Find(model.Id);
                cat.Name = model.Name;
                cat.Description = model.Description;
                
                db.SaveChanges();
                return returnUrl == null ? RedirectToAction("Details", "Households") : RedirectToLocal(returnUrl, RedirectToAction("Details", "Households"));
            }

            return returnUrl == null ? RedirectToAction("Details", "Households") : RedirectToLocal(returnUrl, RedirectToAction("Details", "Households"));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id) {
            Category category = db.Categories.Find(id);
            db.Categories.Remove(category);
            db.SaveChanges();
            return RedirectToAction("Index");
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