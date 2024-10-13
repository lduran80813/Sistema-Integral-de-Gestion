using SIG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SIG.Controllers
{
    [OutputCache(NoStore = true, VaryByParam = "*", Duration = 0)]
    public class HomeController : Controller
    {
        [FiltroSeguridad]
        public ActionResult Index()
        {
            if (TempData["advertencia"] != null)
            {
                var advertencia = TempData["advertencia"].ToString();
                ViewBag.warning = advertencia;
            }
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



    }
}