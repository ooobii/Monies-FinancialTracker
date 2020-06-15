using FinancialTracker_Web.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace FinancialTracker_Web.Controllers
{
    public class BankAccountsController : Controller
    {
        private AppDbContext db = new AppDbContext();

        // GET: BankAccounts
        public ActionResult Index() {
            var bankAccounts = db.BankAccounts.Include(b => b.AccountType).Include(b => b.Owner).Include(b => b.ParentHousehold);
            return View(bankAccounts.ToList());
        }

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

        // GET: BankAccounts/Create
        public ActionResult Create() {
            if( ApplicationUser.GetParentHousehold(User) == null ) { return RedirectToAction("Index", "Home"); }

            ViewBag.AccountTypeId = new SelectList(db.BankAccountTypes, "Id", "Name");
            ViewBag.OwnerId = new SelectList(db.Users, "Id", "FirstName");
            ViewBag.ParentHouseholdId = new SelectList(db.Households, "Id", "Name");
            return View();
        }

        // POST: BankAccounts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "OwnerId,ParentHouseholdId,AccountTypeId,AccountName,StartingBalance,LowBalanceAlertThreshold")] BankAccount bankAccount, string returnUrl) {
            bankAccount.Created = DateTime.Now;
            bankAccount.CurrentBalance = bankAccount.StartingBalance;
            if( ModelState.IsValid ) {

                db.BankAccounts.Add(bankAccount);
                db.SaveChanges();
                return returnUrl == null ? RedirectToAction("Details", "Households") : RedirectToLocal(returnUrl, RedirectToAction("Details", "Households"));
                
            }

            ViewBag.AccountTypeId = new SelectList(db.BankAccountTypes, "Id", "AccountName", bankAccount.AccountTypeId);
            ViewBag.OwnerId = new SelectList(db.Users, "Id", "FirstName", bankAccount.OwnerId);
            ViewBag.ParentHouseholdId = new SelectList(db.Households, "Id", "AccountName", bankAccount.ParentHouseholdId);
            return returnUrl == null ? RedirectToAction("Details", "Households") : RedirectToLocal(returnUrl, RedirectToAction("Details", "Households"));
        }

        // GET: BankAccounts/Edit/5
        public ActionResult Edit(int? id) {
            if( id == null ) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BankAccount bankAccount = db.BankAccounts.Find(id);
            if( bankAccount == null ) {
                return HttpNotFound();
            }
            ViewBag.AccountTypeId = new SelectList(db.BankAccountTypes, "Id", "AccountName", bankAccount.AccountTypeId);
            ViewBag.OwnerId = new SelectList(db.Users, "Id", "FirstName", bankAccount.OwnerId);
            ViewBag.ParentHouseholdId = new SelectList(db.Households, "Id", "AccountName", bankAccount.ParentHouseholdId);
            return View(bankAccount);
        }

        // POST: BankAccounts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,OwnerId,ParentHouseholdId,AccountTypeId,BankId,AccountName,Created,StartingBalance,CurrentBalance,LowBalanceAlertThreshold")] BankAccount bankAccount) {
            if( ModelState.IsValid ) {
                db.Entry(bankAccount).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            ViewBag.AccountTypeId = new SelectList(db.BankAccountTypes, "Id", "AccountName", bankAccount.AccountTypeId);
            ViewBag.OwnerId = new SelectList(db.Users, "Id", "FirstName", bankAccount.OwnerId);
            ViewBag.ParentHouseholdId = new SelectList(db.Households, "Id", "AccountName", bankAccount.ParentHouseholdId);
            return View(bankAccount);
        }

        // GET: BankAccounts/Delete/5
        public ActionResult Delete(int? id) {
            if( id == null ) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BankAccount bankAccount = db.BankAccounts.Find(id);
            if( bankAccount == null ) {
                return HttpNotFound();
            }
            return View(bankAccount);
        }

        // POST: BankAccounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id) {
            BankAccount bankAccount = db.BankAccounts.Find(id);
            db.BankAccounts.Remove(bankAccount);
            db.SaveChanges();
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