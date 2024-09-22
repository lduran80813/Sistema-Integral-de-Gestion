using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SIG.Controllers
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

        public ActionResult Registrousuario()
        {
            ViewBag.Message = "Registrar usuario.";

            return View();
        }

        public ActionResult Recuconn()
        {
            ViewBag.Message = "Registrar usuario.";

            return View();
        }

    }
}