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
                TempData.Add("alertSuccessHouseholdCreated", $"The '{household.Name}' household has been created, and you have been added to the household!");
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
                    TempData.Add("alertSuccessHouseholdEdited", "Changes to household saved!");
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
                TempData.Add("alertSuccessHouseholdDeleted", "The household was successfully deleted.");
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

        [HttpPost]
        public ActionResult Leave() {
            if(!Request.IsAuthenticated) {
                TempData.Add("alertDangerNoUser", "You cannot leave a household if you are not logged in.");
                return RedirectToAction("Index", "Home");
            }
            var houseId = db.Users.Find(User.Identity.GetUserId()).HouseholdId;
            if( houseId == null) {
                TempData.Add("alertDangerNoHousehold", "You must be a member of a household in order to leave one.");
                return RedirectToAction("Index", "Home");
            }
            if( db.Households.Find(houseId).CreatorId == User.Identity.GetUserId() ) {
                TempData.Add("alertDangerNoHousehold", "You cannot leave a household if you are the only member left.");
                return RedirectToAction("Index", "Home");
            }
            if(db.Households.Find(houseId).Members.Count <= 1) {
                TempData.Add("alertDangerNoHousehold", "You cannot leave a household if you are the only member left.");
                return RedirectToAction("Index", "Home");
            }

            db.Users.Find(User.Identity.GetUserId()).HouseholdId = null;
            db.SaveChanges();
            TempData.Add("alertSuccessLeftHousehold", "You have successfully left the household!");
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