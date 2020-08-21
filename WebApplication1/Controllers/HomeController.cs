using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
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
        public ActionResult logOut()
        {
            if (Session["StudentID"] == null && Session["LecturerID"] == null)
            {

                Session["ErrorMessage"] = "Please Login first";
                return RedirectToAction("StudentLogin", "Students");
            }
            else
            {
                return View();
            }
        }
        [HttpPost]
        public ActionResult logOut(string logout)
        {
            Session["StudentID"] = null;
            Session["LecturerID"] = null;
            return RedirectToAction("StudentLogin", "Students");
        }
        

    }
}