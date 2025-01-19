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
