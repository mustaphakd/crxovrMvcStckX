using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using StockExchange.Models;

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace StockExchange.Controllers
{
    /// before adding a new quote, make sure it does not already exist in user list
    /// before removing a quote, make sure to find it in user list.
    /// <summary>
    /// All operation should be conducted on the internal User of the current request
    /// </summary>
    [Authorize]
    [RoutePrefix("Quotes")]
    public class StocksController : Controller
    {
        protected ApplicationDbContext db;

        public StocksController(): this(new ApplicationDbContext())
        {}

        /// <summary>
        /// facilitates mocking param for testing
        /// </summary>
        /// <param name="applicationDbContext"></param>
        internal StocksController(ApplicationDbContext applicationDbContext)
        {
            this.db = applicationDbContext;
        }

        

        // GET: Stocks
        public ActionResult Index()
        {
            var user = GetCurrentUser();
            return View(user.Stocks.ToList());
        }

        // GET: Stocks/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Stock stock = db.Stocks.Find(id);
            if (stock == null)
            {
                return HttpNotFound();
            }
            return View(stock);
        }

        // GET: Stocks/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Stocks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Code")] Stock stock)
        {
            if (stock != null && !String.IsNullOrEmpty(stock.Code))
            {
                //var set = db.Stocks;
                //db.Stocks.Add(stock);
                var user = GetCurrentUser();
                
                if(user.Stocks.Any(stck => stck.Code == stock.Code))
                {
                    ModelState.AddModelError(string.Empty, String.Format("you already have {0} in your watch list", stock.Code));
                    return View(stock);
                }

                var nwStck = new Stock { Code = stock.Code, Owner = user, ApplicationUser_id = user.Id };
                //stock.Owner = user;
                //db.Stocks.Add(nwStck);
                user.Stocks.Add(nwStck);
               // user.Stocks.Add(stock);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(stock);
        }

        // GET: Stocks/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Stock stock = db.Stocks.Find(id);
            if (stock == null)
            {
                return HttpNotFound();
            }
            return View(stock);
        }

        // POST: Stocks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Code")] Stock stock)
        {
            if (ModelState.IsValid)
            {
                db.Entry(stock).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(stock);
        }

        // GET: Stocks/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = GetCurrentUser();
            Stock stock = user.Stocks.FirstOrDefault(stck => stck.StockId == id);
            if (stock == null)
            {
                return HttpNotFound();
            }
            db.Stocks.Remove(stock);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // POST: Stocks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Stock stock = db.Stocks.Find(id);
            db.Stocks.Remove(stock);
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

        private ApplicationUser GetCurrentUser()
        {
            var appUserManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();

            var id = User.Identity.GetUserId();
            var user = db.Users.FirstOrDefault(usr => usr.Id.Equals(id));

            return user;
        }
    }
}
