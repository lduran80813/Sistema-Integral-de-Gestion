using SIG.Models;
using SIG.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SIG.Controllers
{
    public class PlanDeAccionController : Controller
    {
        private readonly PlanDeAccionModel _planDeAccionModel;

        public PlanDeAccionController()
        {
            _planDeAccionModel = new PlanDeAccionModel();
        }

        // Acción GET para registrar un nuevo Plan de Acción
        [HttpGet]
        public ActionResult Registro()
        {

            var empleados = _planDeAccionModel.ListarEmpleados();
            ViewBag.Empleados = empleados ?? new List<SelectListItem>();

            var estadosTarea = _planDeAccionModel.ListarEstadosTarea();
            ViewBag.EstadosTarea = estadosTarea.Any() ? estadosTarea : new List<SelectListItem>();

            var estadosPlan = new List<SelectListItem>
            {
                new SelectListItem { Text = "En Curso", Value = "En Curso" },
                new SelectListItem { Text = "Pendiente", Value = "Pendiente" },
                new SelectListItem { Text = "Completado", Value = "Completado" }
            };
            ViewBag.Estados = estadosPlan;


            var plan = new PlanDeAccion
            {
                Tareas = new List<Tarea> { new Tarea() }  
            };

            return View(plan);
        }

        // Acción POST para registrar un nuevo Plan de Acción con AJAX
        [HttpPost]
        public JsonResult RegistroAjax(PlanDeAccion plan)
        {
            if (ModelState.IsValid)
            {

                foreach (var tarea in plan.Tareas)
                {
                    if (!ModelState.IsValid)
                    {
                        return Json(new { success = false, message = "Por favor, verifique las tareas." });
                    }
                }

                bool resultado = _planDeAccionModel.InsertarPlan(plan);

                if (resultado)
                {
                    return Json(new { success = true, message = "Plan de Acción registrado con éxito." });
                }
                else
                {
                    return Json(new { success = false, message = "Hubo un problema al registrar el Plan de Acción." });
                }
            }
            return Json(new { success = false, message = "Hubo un error con los datos proporcionados." });
        }

        // Acción para ver un Plan de Acción y sus Tareas
        public ActionResult VerTareas(int idPlan)
        {
            var plan = _planDeAccionModel.VerPlan(idPlan);
            if (plan == null)
            {
                return HttpNotFound();
            }

            var tareas = plan.Tareas;

            ViewBag.Plan = plan;
            return View(tareas);
        }

        // Acción para listar los Planes de Acción del usuario
        public ActionResult ListarPlanesUsuario()
        {
            var planes = _planDeAccionModel.ListarPlanesUsuario();
            return View(planes);
        }

        // Acción GET para editar un Plan de Acción
        [HttpGet]
        public ActionResult Editar(int idPlan)
        {
            var plan = _planDeAccionModel.VerPlan(idPlan);
            if (plan == null)
            {
                return HttpNotFound();
            }

            ViewBag.FechaInicio = plan.FechaInicio;
            ViewBag.FechaFinalizacion = plan.FechaFinalizacion;

            var empleados = _planDeAccionModel.ListarEmpleados();
            ViewBag.Empleados = empleados ?? new List<SelectListItem>();

            var estadosPlan = new List<SelectListItem>
    {
        new SelectListItem { Text = "En Curso", Value = "En Curso" },
        new SelectListItem { Text = "Pendiente", Value = "Pendiente" },
        new SelectListItem { Text = "Completado", Value = "Completado" }
    };
            ViewBag.Estados = estadosPlan;

            return View(plan);
        }

        // Acción POST para editar un Plan de Acción
        [HttpPost]
        public ActionResult Editar(PlanDeAccion plan)
        {
            if (ModelState.IsValid)
            {
                bool resultado = _planDeAccionModel.ActualizarPlan(plan);
                if (resultado)
                {
                    TempData["Message"] = "Plan de Acción actualizado con éxito.";
                    return RedirectToAction("ListarPlanesUsuario");
                }
                else
                {
                    ModelState.AddModelError("", "Hubo un problema al actualizar el Plan de Acción.");
                }
            }

            // Volver a cargar los empleados y estados en caso de error
            var empleados = _planDeAccionModel.ListarEmpleados();
            ViewBag.Empleados = empleados ?? new List<SelectListItem>();

            var estadosPlan = new List<SelectListItem>
            {
                new SelectListItem { Text = "En Curso", Value = "En Curso" },
                new SelectListItem { Text = "Pendiente", Value = "Pendiente" },
                new SelectListItem { Text = "Completado", Value = "Completado" }
            };
            ViewBag.Estados = estadosPlan;

            return View(plan);
        }
    }
}
