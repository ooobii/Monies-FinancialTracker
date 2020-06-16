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
    public class BankAccountsController : Controller
    {
        private AppDbContext db = new AppDbContext();


        // GET: BankAccounts/Details/5
        public ActionResult Details(int? id) {
            if( id == null ) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BankAccount bankAccount = db.BankAccounts.Find(id);
            if( bankAccount == null || bankAccount.ParentHousehold.Id != ApplicationUser.GetParentHousehold(User).Id ) {
                return HttpNotFound();
            }
            return View(bankAccount);
        }

        // POST: BankAccounts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "OwnerId,ParentHouseholdId,AccountTypeId,AccountName,StartingBalance,LowBalanceAlertThreshold")] BankAccount bankAccount, string returnUrl) {
            bankAccount.CreatedAt = DateTime.Now;
            if( ModelState.IsValid ) {

                db.BankAccounts.Add(bankAccount);
                db.SaveChanges();
                return returnUrl == null ? RedirectToAction("Details", "Households") : RedirectToLocal(returnUrl, RedirectToAction("Details", "Households"));

            }
            return returnUrl == null ? RedirectToAction("Details", "Households") : RedirectToLocal(returnUrl, RedirectToAction("Details", "Households"));
        }

        // POST: BankAccounts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditBankAccountModel model, string returnUrl) {
            if( ModelState.IsValid ) {
                var bankAccount = db.BankAccounts.First(ba => ba.Id == model.Id);
                bankAccount.AccountName = model.AccountName;
                bankAccount.AccountTypeId = model.AccountTypeId;
                bankAccount.LowBalanceAlertThreshold = model.LowBalanceAlertThreshold;
                db.SaveChanges();
                return returnUrl == null ? RedirectToAction("Details", "Households") : RedirectToLocal(returnUrl, RedirectToAction("Details", "Households"));
            }
            return returnUrl == null ? RedirectToAction("Details", "Households") : RedirectToLocal(returnUrl, RedirectToAction("Details", "Households"));
        }

        // POST: BankAccounts/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, string returnUrl) {
            BankAccount bankAccount = db.BankAccounts.Find(id);
            if( bankAccount.OwnerId == User.Identity.GetUserId() ) {
                db.BankAccounts.Remove(bankAccount);
                db.SaveChanges();
                return returnUrl == null ? RedirectToAction("Details", "Households") : RedirectToLocal(returnUrl, RedirectToAction("Details", "Households"));
            }
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