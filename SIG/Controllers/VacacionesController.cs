using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SIG.Controllers
{
    public class VacacionesController
    {
        private readonly Vacaciones _vacacionesService;

        public VacacionesController()
        {
            _vacacionesService = new VacacionesService();
        }

        // Mostrar formulario para solicitar vacaciones
        public IActionResult Solicitar()
        {
            return View();
        }

        // Procesar la solicitud de vacaciones
        [HttpPost]
        public IActionResult Solicitar(VacacionesModel modelo)
        {
            if (ModelState.IsValid)
            {
                var resultado = _vacacionesService.SolicitarVacaciones(modelo);
                if (!resultado.Exitoso)
                {
                    ViewBag.Error = resultado.MensajeError;
                    return View(modelo); // Regresar con error
                }

                return RedirectToAction("Historial"); // Redirigir al historial
            }

            return View(modelo); // Devolver vista con errores de validación
        }

        // Mostrar historial de solicitudes
        public IActionResult Historial(int empleadoId)
        {
            var historial = _vacacionesService.ObtenerHistorial(empleadoId);
            return View(historial);
        }

        // Mostrar solicitudes pendientes para el supervisor
        public IActionResult RevisarPendientes(int supervisorId)
        {
            var solicitudes = _vacacionesService.ObtenerPendientes(supervisorId);
            return View(solicitudes);
        }

        // Aprobar o rechazar solicitudes
        [HttpPost]
        public IActionResult ResponderSolicitud(int solicitudId, string estado, string comentario)
        {
            var resultado = _vacacionesService.ResponderSolicitud(solicitudId, estado, comentario);
            if (!resultado.Exitoso)
            {
                TempData["Error"] = resultado.MensajeError;
            }
            return RedirectToAction("RevisarPendientes", new { supervisorId = User.Identity.GetUserId() });
        }
    }
}