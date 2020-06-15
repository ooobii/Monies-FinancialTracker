using FinancialTracker_Web.Models;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace FinancialTracker_Web.Controllers
{
    public class TransactionsController : Controller
    {
        private AppDbContext db = new AppDbContext();

        // GET: Transactions
        public ActionResult Index() {
            var transactions = db.Transactions.Include(t => t.CategoryItem).Include(t => t.ParentAccount).Include(t => t.TransactionType);
            return View(transactions.ToList());
        }

        // GET: Transactions/Details/5
        public ActionResult Details(int? id) {
            if( id == null ) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if( transaction == null ) {
                return HttpNotFound();
            }
            return View(transaction);
        }

        // GET: Transactions/Create
        public ActionResult Create() {
            ViewBag.CategoryItemId = new SelectList(db.CategoryItems, "Id", "AccountName");
            ViewBag.ParentAccountId = new SelectList(db.BankAccounts, "Id", "OwnerId");
            ViewBag.TransactionTypeId = new SelectList(db.TransactionTypes, "Id", "AccountName");
            return View();
        }

        // POST: Transactions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ParentAccountId,TransactionTypeId,CategoryItemId,OwnerId,Memo,Amount,CreatedAt")] Transaction transaction) {
            if( ModelState.IsValid ) {
                db.Transactions.Add(transaction);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CategoryItemId = new SelectList(db.CategoryItems, "Id", "AccountName", transaction.CategoryItemId);
            ViewBag.ParentAccountId = new SelectList(db.BankAccounts, "Id", "OwnerId", transaction.ParentAccountId);
            ViewBag.TransactionTypeId = new SelectList(db.TransactionTypes, "Id", "AccountName", transaction.TransactionTypeId);
            return View(transaction);
        }

        // GET: Transactions/Edit/5
        public ActionResult Edit(int? id) {
            if( id == null ) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if( transaction == null ) {
                return HttpNotFound();
            }
            ViewBag.CategoryItemId = new SelectList(db.CategoryItems, "Id", "AccountName", transaction.CategoryItemId);
            ViewBag.ParentAccountId = new SelectList(db.BankAccounts, "Id", "OwnerId", transaction.ParentAccountId);
            ViewBag.TransactionTypeId = new SelectList(db.TransactionTypes, "Id", "AccountName", transaction.TransactionTypeId);
            return View(transaction);
        }

        // POST: Transactions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ParentAccountId,TransactionTypeId,CategoryItemId,OwnerId,Memo,Amount,CreatedAt")] Transaction transaction) {
            if( ModelState.IsValid ) {
                db.Entry(transaction).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CategoryItemId = new SelectList(db.CategoryItems, "Id", "AccountName", transaction.CategoryItemId);
            ViewBag.ParentAccountId = new SelectList(db.BankAccounts, "Id", "OwnerId", transaction.ParentAccountId);
            ViewBag.TransactionTypeId = new SelectList(db.TransactionTypes, "Id", "AccountName", transaction.TransactionTypeId);
            return View(transaction);
        }

        // GET: Transactions/Delete/5
        public ActionResult Delete(int? id) {
            if( id == null ) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if( transaction == null ) {
                return HttpNotFound();
            }
            return View(transaction);
        }

        // POST: Transactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id) {
            Transaction transaction = db.Transactions.Find(id);
            db.Transactions.Remove(transaction);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing) {
            if( disposing ) {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}