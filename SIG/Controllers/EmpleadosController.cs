using SIG.Entidades;
using SIG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SIG.Controllers
{
    public class EmpleadosController : Controller
    {
        private readonly EmpleadosModel _empleadosModel;

        public EmpleadosController()
        {
            _empleadosModel = new EmpleadosModel();
        }


        // GET: Empleados
        public ActionResult Registro()
        {
            return View();
        }


        [HttpGet]
        public ActionResult ListarCapacitaciones()
        {
            var planes = _empleadosModel.ListarCapacitaciones();
            return View(planes);
        }

        // Acción GET para registrar un nuevo Plan de Acción
        [HttpGet]
        public ActionResult RegistroCapacitacion()
        {
            var empleados = _empleadosModel.ListarEmpleados();
            ViewBag.Empleados = empleados ?? new List<SelectListItem>();


            var capacitacion = new EmpCapacitacion
            {
                Participantes = new List<EmpCapacitacionParticipantes> { new EmpCapacitacionParticipantes() }
            };

            return View(capacitacion);
        }

        // Acción POST para registrar un nuevo Plan de Acción
        [HttpPost]
        public JsonResult RegistroAjax(EmpCapacitacion capacitacion)
        {
            if (ModelState.IsValid)
            {
                bool resultado = _empleadosModel.InsertarCapacitacion(capacitacion);

                if (resultado)
                {
                    return Json(new { success = true, message = "Plan de Acción registrado con éxito." });
                }
                else
                {
                    return Json(new { success = false, message = "Hubo un problema al registrar el Plan de Acción." });
                }
            }
            return Json(new { success = false, message = "Datos proporcionados no válidos." });
        }

        [HttpGet]
        public ActionResult VerParticipantes(int idCapacitacion)
        {
            var capacitacion = _empleadosModel.VerCapacitacion(idCapacitacion);
            if (capacitacion == null)
            {
                return HttpNotFound();
            }

            ViewBag.Capacitacion = capacitacion;
            return View(capacitacion.Participantes);
        }

        [HttpGet]
        public ActionResult EditarCapacitacion(int idCapacitacion)
        {
            var plan = _empleadosModel.VerCapacitacion(idCapacitacion);
            if (plan == null)
            {
                return HttpNotFound();
            }

            var empleados = _empleadosModel.ListarEmpleados();
            ViewBag.Empleados = empleados ?? new List<SelectListItem>();

            return View(plan);
        }

        // Acción POST para editar un Plan de Acción
        [HttpPost]
        public ActionResult EditarCapacitacion(EmpCapacitacion capacitacion)
        {
            if (ModelState.IsValid)
            {
                bool resultado = _empleadosModel.ActualizarCapacitacion(capacitacion);
                if (resultado)
                {
                    TempData["Message"] = "Capacitación actualizada con éxito.";
                    return RedirectToAction("ListarCapacitaciones");
                }
                else
                {
                    ModelState.AddModelError("", "Hubo un problema al actualizar la capacitación.");
                }
            }

            var empleados = _empleadosModel.ListarEmpleados();
            ViewBag.Empleados = empleados ?? new List<SelectListItem>();

            return View(capacitacion);
        }


    }
}