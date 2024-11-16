using SIG.BaseDatos;
using SIG.Entidades;
using SIG.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Mail;
using System.Net;
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



        public ActionResult ActualizarEntrega(int id)
        {
            var entrega = entregas.ObtenerEntregaPorId(id); 
            if (entrega == null)
            {
                return HttpNotFound(); 
            }

            return View(entrega);
        }

        [HttpPost]
        public ActionResult ActualizarEntrega(Entrega entrega)
        {
            if (ModelState.IsValid)
            {
                bool resultado = entregas.ActualizarEntrega(entrega);
                if (resultado)
                {
                    return RedirectToAction("ListarEntregas");
                }
                else
                {
                    ModelState.AddModelError("", "No se pudo actualizar la entrega.");
                }
            }

            return View(entrega); 
        }



        [HttpGet]
        public ActionResult EnviarNotificacion(int id)
        {
            var entrega = entregas.ObtenerPorId(id);
            if (entrega == null)
            {
                TempData["Error"] = "No se encontró la entrega especificada.";
                return RedirectToAction("ListarEntregas");
            }

            try
            {
                string cuerpoCorreo = $@"
            <h3>Actualización de Entrega</h3>
            <p>Estimado Cliente,</p>
            <p>Le informamos sobre los detalles de su entrega:</p>
            <table>
                <tr><td><strong>Número de Pedido:</strong></td><td>{entrega.pedido_id}</td></tr>
                <tr><td><strong>Fecha de Entrega:</strong></td><td>{entrega.fecha_entrega?.ToString("dd/MM/yyyy") ?? "N/A"}</td></tr>
                <tr><td><strong>Dirección de Entrega:</strong></td><td>{entrega.direccion_entrega}</td></tr>
                <tr><td><strong>Estado de la Entrega:</strong></td><td>{entrega.EstadoEntrega}</td></tr>
                <tr><td><strong>Nombre del Destinatario:</strong></td><td>{entrega.NombreDestinatario}</td></tr>
            </table>
            <p>Gracias por utilizar nuestros servicios.</p>
        ";

                entregas.EnviarNotificacionCorreo(entrega.CorreoElectronico, "Detalles de su Entrega", cuerpoCorreo);
                TempData["Mensaje"] = "Notificación enviada exitosamente al cliente.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Hubo un error al enviar la notificación: {ex.Message}";
            }

            return RedirectToAction("ListarEntregas");
        }


    }
}