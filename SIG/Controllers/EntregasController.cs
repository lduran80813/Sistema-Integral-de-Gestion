using SIG.BaseDatos;
using SIG.Entidades;
using SIG.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SIG.Controllers
{
    public class EntregasController : Controller
    {
        EntregasModel entregas = new EntregasModel();

        [HttpGet]
        public ActionResult Registrar()
        {
            return View();
        }


        [HttpPost]
        public ActionResult Registrar(Entrega entrega)
        {
            if (ModelState.IsValid)
            {
                bool result = entregas.RegistrarEntrega(entrega);

                if (!result) 
                {
                  
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "No se pudo registrar la entrega. Intente nuevamente.");
                }
            }
            return View(entrega);
        }

        public ActionResult ListarEntregas()
        {
            var entrega = entregas.ListarEntregas();
            return View(entrega);
        }
    }



}