using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SIG.Controllers
{
    public class ContabilidadController : Controller
    {
        // GET: Contabilidad
        public ActionResult Registro_Ventas()
        {
            return View();
        }

        public ActionResult Generar_Factura()
        {
            return View();
        }

        public ActionResult CxP()
        {
            return View();
        }
    }
}