using SIG.BaseDatos;
using SIG.Entidades;
using SIG.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace SIG.Controllers
{
    public class EmpleadosController : Controller
    {
        private readonly EmpleadosModel _empleadosModel;
        private readonly VacacionesModel _vacacionesService;

        public EmpleadosController()
        {
            _empleadosModel = new EmpleadosModel();
            _vacacionesService = new VacacionesModel();
        }


        // GET: Empleados
        public ActionResult Registro()
        {
            return View();
        }

        public ActionResult Solicitar()
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


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Solicitar(string correoElectronico, DateTime fechaInicio, DateTime fechaFin, string observaciones)
        {
            try
            {

                if (string.IsNullOrEmpty(correoElectronico))
                {
                    TempData["ErrorMessage"] = "El correo electrónico es obligatorio.";
                    return RedirectToAction("Solicitar");
                }

                int empleadoId = ObtenerEmpleadoId(correoElectronico);

                if (empleadoId == 0)
                {
                    TempData["ErrorMessage"] = "No se encontró un empleado con ese correo electrónico.";
                    return RedirectToAction("Solicitar");
                }

                using (var context = new SistemaIntegralGestionEntities())
                {
                    var solicitudExistente = context.SolicitudesVacaciones
                        .FirstOrDefault(s => s.empleado_id == empleadoId && s.estado == "Pendiente");

                    if (solicitudExistente != null)
                    {
                        TempData["ErrorMessage"] = "Ya tienes una solicitud de vacaciones en proceso. No puedes realizar una nueva solicitud hasta que se resuelva la actual.";
                        return RedirectToAction("Solicitar");
                    }
                }


                int diasSolicitados = (fechaFin - fechaInicio).Days + 1;

                var result = _vacacionesService.SolicitarVacaciones(empleadoId, fechaInicio, fechaFin, diasSolicitados, observaciones, null);

                if (result)
                {
                    TempData["SuccessMessage"] = "Solicitud de vacaciones realizada con éxito.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Error al procesar la solicitud de vacaciones.";
                }

                return RedirectToAction("Solicitar"); 
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Ocurrió un error al procesar la solicitud: " + ex.Message;
                return RedirectToAction("Solicitar");
            }
        }


        public int ObtenerEmpleadoId(string correoElectronico)
        {
            using (var context = new SistemaIntegralGestionEntities())
            {
                var empleado = context.Empleado
                    .FirstOrDefault(e => e.correo_electronico == correoElectronico);

                return empleado?.id ?? 0; 
            }
        }

        public ActionResult RevisarPendientes()
        {
            using (var context = new SistemaIntegralGestionEntities())
            {
                var solicitudes = context.SolicitudesVacaciones.ToList(); 
                return View(solicitudes);
            }
        }

        public ActionResult AprobarRechazar(int solicitudId)
        {
            using (var context = new SistemaIntegralGestionEntities())
            {
                var solicitud = context.SolicitudesVacaciones
                    .FirstOrDefault(s => s.id == solicitudId);

                if (solicitud == null)
                {
                    TempData["ErrorMessage"] = "La solicitud de vacaciones no existe.";
                    return RedirectToAction("VerSolicitudesVacaciones");
                }

                return View(solicitud);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AprobarRechazar(int solicitudId, string estado, string motivoRechazo)
        {
            try
            {

                if (string.IsNullOrEmpty(estado) || (estado != "Aprobado" && estado != "Rechazado"))
                {
                    TempData["ErrorMessage"] = "Estado no válido.";
                    return RedirectToAction("VerSolicitudesVacaciones");
                }

                string correoAdmin = "admin@dominio.com"; 
                int administradorId = ObtenerEmpleadoId(correoAdmin); 

                var resultado = _vacacionesService.AprobarRechazarVac(solicitudId, estado, administradorId, motivoRechazo);

                if (resultado)
                {
                    TempData["SuccessMessage"] = "Solicitud procesada con éxito.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Error al procesar la solicitud.";
                }

                return RedirectToAction("VerSolicitudesVacaciones");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Ocurrió un error: " + ex.Message;
                return RedirectToAction("VerSolicitudesVacaciones");
            }
        }
    }
    }
