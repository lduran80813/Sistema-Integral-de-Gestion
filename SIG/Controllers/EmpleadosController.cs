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
    public class EmpleadosController : Controller
    {
        private readonly VacacionesModel _vacacionesService;

        public EmpleadosController()
        {
            _vacacionesService = new VacacionesModel();
        }

        /// <summary>
        /// Muestra el formulario para solicitar vacaciones.
        /// </summary>
        public ActionResult Solicitar(string correo)
        {
            try
            {
                using (var context = new SistemaIntegralGestionEntities())
                {
                    // Buscar empleado por correo
                    var empleado = context.Empleado.FirstOrDefault(e => e.correo_electronico == correo);
                    if (empleado == null)
                    {
                        TempData["MensajeError"] = "No se encontró un empleado con el correo proporcionado.";
                        return RedirectToAction("Solicitar");
                    }

                    // Crear el modelo para la vista
                    var model = new Vacaciones
                    {
                        EmpleadoId = empleado.id,
                        CorreoEmpleado = correo
                    };

                    return View(model); // Renderizar la vista con el modelo
                }
            }
            catch (Exception ex)
            {
                TempData["MensajeError"] = $"Error al buscar el empleado: {ex.Message}";
                return RedirectToAction("Solicitar");
            }
        }

        /// <summary>
        /// Procesa la solicitud de vacaciones enviada por el formulario.
        /// </summary>
        [HttpPost]
        public ActionResult Solicitar(string correoEmpleado, DateTime fechaInicio, DateTime fechaFin, string observaciones)
        {
            try
            {
                if (string.IsNullOrEmpty(correoEmpleado))
                {
                    TempData["MensajeError"] = "El correo del empleado es obligatorio.";
                    return RedirectToAction("Solicitar");
                }

                // Llama al servicio para procesar la solicitud de vacaciones
                string resultado = _vacacionesService.SolicitarVacaciones(correoEmpleado, fechaInicio, fechaFin, observaciones);

                if (resultado == "Éxito")
                {
                    TempData["MensajeExito"] = "Solicitud registrada correctamente.";
                }
                else
                {
                    TempData["MensajeError"] = resultado; // Mostrar el error devuelto por el servicio
                }

                return RedirectToAction("Solicitar");
            }
            catch (Exception ex)
            {
                TempData["MensajeError"] = $"Error: {ex.Message}";
                return RedirectToAction("Solicitar");
            }
        }



    }
}
