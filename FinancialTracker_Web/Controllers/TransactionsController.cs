using System;
using FinancialTracker_Web.Models;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace FinancialTracker_Web.Controllers
{
    public class TransactionsController : Controller
    {
        private AppDbContext db = new AppDbContext();


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ParentAccountId,TransactionTypeId,CategoryItemId,Name,Memo,Amount,CreatedAt,OccuredAt")] Transaction transaction, string returnUrl) {
            transaction.CreatedAt = DateTime.Now;
            transaction.OwnerId = User.Identity.GetUserId();

            if( ModelState.IsValid ) {
                db.Transactions.Add(transaction);
                db.SaveChanges();
            }
            return returnUrl == null ? RedirectToAction("Details", "Households") : RedirectToLocal(returnUrl, RedirectToAction("Details", "Households"));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditTransactionModel model, string returnUrl) {
            if( ModelState.IsValid ) {
                var trans = db.Transactions.Find(model.Id);
                if( trans != null ) {
                    trans.TransactionTypeId = model.TransactionTypeId;
                    trans.CategoryItemId = model.CategoryItemId;
                    trans.Memo = model.Memo;
                    trans.Amount = model.Amount;
                    trans.OccuredAt = model.OccuredAt;

                    db.SaveChanges();
                }
            }
            return returnUrl == null ? RedirectToAction("Details", "Households") : RedirectToLocal(returnUrl, RedirectToAction("Details", "Households"));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, string returnUrl) {
            Transaction transaction = db.Transactions.Find(id);
            if( transaction != null ) {
                db.Transactions.Remove(transaction);
                db.SaveChanges();
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