using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Buggity.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            //if (!this.User.Identity.IsAuthenticated)
            //{




            //    if (User.IsInRole("Admin"))
            //    {
            //        return RedirectToAction("AdminDashboard", "Projects");
            //    }

            //    if (User.IsInRole("Developer"))
            //    {
            //        return RedirectToAction("DeveloperDashboard", "Projects");
            //    }

            //    if (User.IsInRole("PM"))
            //    {
            //        return RedirectToAction("PmDashboard", "Projects");
            //    }
            //    if (User.IsInRole("Submitter"))
            //    {
            //        return RedirectToAction("SubmitterDashboard", "Tickets");
            //    }
            //}



            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}