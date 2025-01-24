using SIG.BaseDatos;
using SIG.Entidades;
using SIG.Models;
using System;
using System.Collections.Generic;
using System.Data;
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

        // Acción GET para registrar una nueva capacitación
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

        // Acción POST para registrar una nueva capacitación
        [HttpPost]
        public JsonResult RegistroAjax(EmpCapacitacion capacitacion)
        {
            if (ModelState.IsValid)
            {
                bool resultado = _empleadosModel.InsertarCapacitacion(capacitacion);

                if (resultado)
                {
                    return Json(new { success = true, message = "Capacitación registrada con éxito." });
                }
                else
                {
                    return Json(new { success = false, message = "Hubo un problema al registrar la capacitación." });
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

        // Acción POST para editar una capacitación
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
        public ActionResult Solicitar(string cedulaEmpleado, DateTime fechaInicio, DateTime fechaFin, string observaciones)
        {
            try
            {
                if (string.IsNullOrEmpty(cedulaEmpleado))
                {
                    TempData["ErrorMessage"] = "La cédula es obligatoria.";
                    return RedirectToAction("Solicitar");
                }


                int empleadoId = ObtenerEmpleadoIdPorCedula(cedulaEmpleado);

                if (empleadoId == 0)
                {
                    TempData["ErrorMessage"] = "No se encontró un empleado con esa cédula.";
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


                bool result = _vacacionesService.SolicitarVacaciones(empleadoId, fechaInicio, fechaFin, diasSolicitados, observaciones, null);

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

        private int ObtenerEmpleadoIdPorCedula(string cedulaEmpleado)
        {
            using (var context = new SistemaIntegralGestionEntities())
            {
                var empleado = context.Empleado.FirstOrDefault(e => e.numero_identificacion == cedulaEmpleado);
                return empleado != null ? empleado.id : 0;
            }
        }

        public ActionResult Historial()
        {
            var solicitudes = _vacacionesService.ListarTodasVacaciones();
            return View(solicitudes);
        }


        public ActionResult HistorialPorUsuario()
        {
            var idUsuario = Session["IdUsuario"] as int?;

            if (idUsuario == null)
            {
                ViewBag.msj = "No estás autenticado.";
                return RedirectToAction("Index", "Home");
            }

            List<SIG.Entidades.Vacaciones> solicitudes = new List<SIG.Entidades.Vacaciones>();

            using (var context = new SistemaIntegralGestionEntities())
            {

                var idParametro = new SqlParameter("@id_empleado", SqlDbType.Int) { Value = idUsuario };

                solicitudes = context.Database.SqlQuery<SIG.Entidades.Vacaciones>(
                    "EXEC ObtenerHistorialSolicitudesVacaciones @id_empleado", idParametro
                ).ToList();
            }

            return View(solicitudes);
        }

        [HttpPost]
        public ActionResult AprobarRechazarVacaciones(int solicitudId, string estado, string motivoRechazo)
        {

            var rolUsuario = Session["RolUsuario"] as int?;

            if (rolUsuario == null || rolUsuario != 1)
            {
                ViewBag.msj = "No tienes permisos para aprobar o rechazar solicitudes de vacaciones.";
                return RedirectToAction("Index", "Home");
            }


            using (var context = new SistemaIntegralGestionEntities())
            {
                var solicitud = context.SolicitudesVacaciones.FirstOrDefault(s => s.id == solicitudId);
                if (solicitud != null)
                {
                    solicitud.estado = estado;


                    if (estado == "Aprobado")
                    {
                        solicitud.fecha_aprobacion = DateTime.Now;
                        solicitud.aprobado_por = (int)Session["IdUsuario"];
                    }
                    else
                    {

                        if (!string.IsNullOrEmpty(motivoRechazo))
                        {
                            solicitud.motivo_rechazo = motivoRechazo;
                        }
                        else
                        {
                            solicitud.motivo_rechazo = "Rechazado por administración sin motivo específico";
                        }
                    }

                    context.SaveChanges();
                    ViewBag.msj = "La solicitud de vacaciones ha sido " + estado.ToLower() + " correctamente.";
                }
                else
                {
                    ViewBag.msj = "La solicitud no existe.";
                }
            }

            return RedirectToAction("Historial", "Empleados");
        }
    }
}
