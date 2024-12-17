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
        private readonly VacacionesModel _vacacionesService;

        public EmpleadosController()
        {
            _vacacionesService = new VacacionesModel();
        }

        public ActionResult Solicitar()
        {
            return View();
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
