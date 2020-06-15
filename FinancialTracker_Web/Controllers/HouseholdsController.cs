using FinancialTracker_Web.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace FinancialTracker_Web.Controllers
{
    public class HouseholdsController : Controller
    {
        private AppDbContext db = new AppDbContext();

        // GET: Households
        public ActionResult Index() {
            return View(db.Households.ToList());
        }

        // GET: Households/Details/5
        public ActionResult Details() {
            var house = ApplicationUser.GetFromDb(User, db).Household;
            if( house == null ) {
                return RedirectToAction("Index", "Home");
            }
            return View(house);
        }

        // GET: Households/Create
        public ActionResult Create() {
            return View();
        }

        // POST: Households/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Name,Greeting")] Household household) {
            if( ModelState.IsValid ) {
                household.Members.Add(ApplicationUser.GetFromDb(User, db));
                household.CreatorId = User.Identity.GetUserId();
                household.CreatedAt = DateTime.Now;

                db.Households.Add(household);
                db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }

            return View(household);
        }

        // GET: Households/Edit/5
        public ActionResult Edit(int? id) {
            if( id == null ) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Household household = db.Households.Find(id);
            if( household == null ) {
                return HttpNotFound();
            }
            return View(household);
        }

        // POST: Households/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,AccountName,Greeting")] Household household) {
            if( ModelState.IsValid ) {
                db.Entry(household).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index", "Home");
        }

        // GET: Households/Delete/5
        public ActionResult Delete(int? id) {
            if( id == null ) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Household household = db.Households.Find(id);
            if( household == null ) {
                return HttpNotFound();
            }
            return View(household);
        }

        // POST: Households/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id) {
            Household household = db.Households.Find(id);
            if( household != null ) {
                household.Members.Clear();

                db.Households.Remove(household);
                db.SaveChanges();
            }
            return RedirectToAction("Index", "Home");
        }

        protected override void Dispose(bool disposing) {
            if( disposing ) {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}