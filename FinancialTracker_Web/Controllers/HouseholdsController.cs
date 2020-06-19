using FinancialTracker_Web.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace FinancialTracker_Web.Controllers
{
    [Authorize]
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


        [HttpGet]
        public ActionResult ProcessInvitation(int inviteId) {
            var inviteResult = Invitation.ProcessInvite(db, inviteId, User);
            switch( inviteResult ) {
                case Invitation.InviteResult.FailureNoInvite:
                    TempData.Add("alertDangerInvite", "We were unable to locate the invite you requested. Please try again.");
                    break;

                case Invitation.InviteResult.FailureInvalidInvite:
                    TempData.Add("alertInfoInvite", "We're sorry, but this invite has expired, or cannot be processed anymore!");
                    break;

                case Invitation.InviteResult.FailureBadCaller:
                    TempData.Add("alertDangerInvite", "We're sorry, but you are not the intended recipient of this invite. Please try again.");
                    break;
                case Invitation.InviteResult.Success:
                    TempData.Add("alertSuccessInvite", $"Invitation Accepted! Welcome to the {db.Invitations.Find(inviteId).ParentHousehold.Name} household!");
                    break;
                default:
                    break;
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