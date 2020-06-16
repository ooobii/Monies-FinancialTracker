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
        

        public ActionResult Details() {
            var house = ApplicationUser.GetFromDb(User, db).Household;
            if( house == null ) {
                return RedirectToAction("Index", "Home");
            }
            return View(house);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Name,Greeting,CreatorId")] Household household) {
            household.CreatedAt = DateTime.Now;
            if( ModelState.IsValid ) {
                household.Members.Add(ApplicationUser.GetFromDb(User, db));
                household.CreatorId = User.Identity.GetUserId();

                db.Households.Add(household);
                db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Details");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditHouseholdModel model, string returnUrl) {
            if( ModelState.IsValid ) {
                var house = db.Households.Find(model.Id);
                if( house != null && User.Identity.GetUserId() == house.CreatorId ) {
                    house.Name = model.Name;
                    house.Greeting = model.Greeting;
                    db.SaveChanges();
                }
            }
            return returnUrl == null ? RedirectToAction("Details", "Households") : RedirectToLocal(returnUrl, RedirectToAction("Details", "Households"));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id) {
            Household household = db.Households.Find(id);
            if( household != null ) {
                household.Members.Clear();
                household.Categories.Clear();
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