using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FinancialTracker_Web.Models;

namespace FinancialTracker_Web.Controllers
{
    public class BankAccountTypesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: BankAccountTypes
        public ActionResult Index()
        {
            return View(db.BankAccountTypes.ToList());
        }

        // GET: BankAccountTypes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BankAccountType bankAccountType = db.BankAccountTypes.Find(id);
            if (bankAccountType == null)
            {
                return HttpNotFound();
            }
            return View(bankAccountType);
        }

        // GET: BankAccountTypes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: BankAccountTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name")] BankAccountType bankAccountType)
        {
            if (ModelState.IsValid)
            {
                db.BankAccountTypes.Add(bankAccountType);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(bankAccountType);
        }

        // GET: BankAccountTypes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BankAccountType bankAccountType = db.BankAccountTypes.Find(id);
            if (bankAccountType == null)
            {
                return HttpNotFound();
            }
            return View(bankAccountType);
        }

        // POST: BankAccountTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name")] BankAccountType bankAccountType)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bankAccountType).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(bankAccountType);
        }

        // GET: BankAccountTypes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BankAccountType bankAccountType = db.BankAccountTypes.Find(id);
            if (bankAccountType == null)
            {
                return HttpNotFound();
            }
            return View(bankAccountType);
        }

        // POST: BankAccountTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BankAccountType bankAccountType = db.BankAccountTypes.Find(id);
            db.BankAccountTypes.Remove(bankAccountType);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
