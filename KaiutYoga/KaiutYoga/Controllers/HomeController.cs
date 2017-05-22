using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KaiutYoga.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";

            if (!Request.IsAuthenticated)
                return RedirectToAction("Login", "Account");
            return View();
        }

        public ActionResult Aulas()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Entries()
        {
            ViewBag.Message = "Contém os cadastros básicos para utilização do sistema.";

            return View();
        }
    }
}
