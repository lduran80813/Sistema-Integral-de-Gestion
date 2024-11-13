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


        // Método GET para actualizar la entrega
        public ActionResult ActualizarEntrega(int id)
        {
            var entrega = entregas.ObtenerEntregaPorId(id); // Llama a un método para obtener los detalles de la entrega por ID
            if (entrega == null)
            {
                return HttpNotFound(); // Si no se encuentra la entrega, retorna un error 404
            }

            return View(entrega); // Pasa el modelo a la vista
        }

        [HttpPost]
        public ActionResult ActualizarEntrega(Entrega entrega)
        {
            if (ModelState.IsValid)
            {
                bool resultado = entregas.ActualizarEntrega(entrega); // Actualiza la entrega
                if (resultado)
                {
                    return RedirectToAction("ListarEntregas"); // Redirige a la lista de entregas si la actualización fue exitosa
                }
                else
                {
                    ModelState.AddModelError("", "No se pudo actualizar la entrega.");
                }
            }

            return View(entrega); // Vuelve a mostrar el formulario si la validación falla
        }



    }
}