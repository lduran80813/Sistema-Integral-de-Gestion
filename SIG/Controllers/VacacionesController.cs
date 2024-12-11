using DocumentFormat.OpenXml.Spreadsheet;
using SIG.Entidades;
using SIG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace SIG.Controllers
{
    public class VacacionesController : Controller
    {
        private readonly VacacionesModel _vacacionesService;

        public VacacionesController()
        {
            _vacacionesService = new VacacionesModel();
        }

        // Acción para mostrar el formulario de solicitud de vacaciones
        public ActionResult Solicitar()
        {
            return View();
        }

        // Acción para procesar la solicitud de vacaciones
        [HttpPost]
        public ActionResult Solicitar(Vacaciones modelo)
        {
            if (ModelState.IsValid)
            {
                var resultado = _vacacionesService.SolicitarVacaciones(modelo);

                if (resultado.Exitoso)
                {
                    TempData["Mensaje"] = "Solicitud enviada con éxito.";
                    return RedirectToAction("Historial");
                }

                ViewBag.Error = resultado.MensajeError;
            }

            return View(modelo);
        }

        // Acción para mostrar el historial de solicitudes de vacaciones
        public ActionResult Historial()
        {
            int empleadoId = ObtenerEmpleadoIdActual(); // Asume un método para obtener el ID del empleado actual
            var historial = _vacacionesService.ObtenerHistorial(empleadoId);
            return View(historial);
        }

        // Acción para mostrar las solicitudes pendientes para el supervisor
        public ActionResult RevisarPendientes()
        {
            int supervisorId = ObtenerSupervisorIdActual(); // Asume un método para obtener el ID del supervisor actual
            var pendientes = _vacacionesService.ObtenerPendientes(supervisorId);
            return View(pendientes);
        }

        // Acción para responder una solicitud de vacaciones
        [HttpPost]
        public ActionResult ResponderSolicitud(int solicitudId, string estado, string comentario)
        {
            var resultado = _vacacionesService.ResponderSolicitud(solicitudId, estado, comentario);

            if (resultado.Exitoso)
            {
                TempData["Mensaje"] = "Solicitud respondida con éxito.";
            }
            else
            {
                TempData["Error"] = resultado.MensajeError;
            }

            return RedirectToAction("RevisarPendientes");
        }

        // Métodos auxiliares para obtener IDs del empleado o supervisor actual
        private int ObtenerEmpleadoIdActual()
        {
            // Simulación: devuelve un ID estático. Reemplazar con lógica de sesión o autenticación.
            return 1; // Ejemplo
        }

        private int ObtenerSupervisorIdActual()
        {
            // Simulación: devuelve un ID estático. Reemplazar con lógica de sesión o autenticación.
            return 2; // Ejemplo
        }
    }
}