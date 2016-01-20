using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StockExchange.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if (Request != null && Request.IsAuthenticated == true)
                return RedirectToAction("Index", "Stocks");
            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }
    }
}