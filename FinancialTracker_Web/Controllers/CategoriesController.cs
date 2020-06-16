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

        // GET: Categories
        public ActionResult Index() {
            var categories = db.Categories.Include(c => c.ParentHousehold);
            return View(categories.ToList());
        }

        // GET: Categories/Details/5
        public ActionResult Details(int? id) {
            if( id == null ) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Find(id);
            if( category == null ) {
                return HttpNotFound();
            }
            return View(category);
        }

        // GET: Categories/Create
        public ActionResult Create() {
            ViewBag.ParentHouseholdId = new SelectList(db.Households, "Id", "AccountName");
            return View();
        }

        // POST: Categories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
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

        // GET: Categories/Edit/5
        public ActionResult Edit(int? id) {
            if( id == null ) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Find(id);
            if( category == null ) {
                return HttpNotFound();
            }
            ViewBag.ParentHouseholdId = new SelectList(db.Households, "Id", "AccountName", category.ParentHouseholdId);
            return View(category);
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ParentHouseholdId,AccountName,Description,AmountBudgeted")] Category category) {
            if( ModelState.IsValid ) {
                db.Entry(category).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ParentHouseholdId = new SelectList(db.Households, "Id", "AccountName", category.ParentHouseholdId);
            return View(category);
        }

        // GET: Categories/Delete/5
        public ActionResult Delete(int? id) {
            if( id == null ) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Find(id);
            if( category == null ) {
                return HttpNotFound();
            }
            return View(category);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id) {
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