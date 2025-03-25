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

        //[HttpGet]
        //public ActionResult Registrar()
        //{
        //    var entrega = new Entrega();

        //    if (!entrega.FechaEntrega.HasValue)
        //    {
        //        entrega.FechaEntrega = DateTime.Today;
        //    }

        //    return View(entrega);
        //}

        [HttpGet]
        public ActionResult Registrar()
        {
            var entrega = new Entrega();


            using (var context = new SistemaIntegralGestionEntities())
            {
                var clientes = context.Venta_Cliente
                                      .Select(c => new { c.id, c.nombre })
                                      .ToList();


                ViewBag.Clientes = new SelectList(clientes, "id", "nombre");
            }

            if (!entrega.FechaEntrega.HasValue)
            {
                entrega.FechaEntrega = DateTime.Today;
            }

            return View(entrega);
        }



        [HttpPost]
        public ActionResult Registrar(Entrega entrega)
        {
            if (!entrega.PedidoId.HasValue || !entregas.ExisteNumeroPedido(entrega.PedidoId))
            {
                TempData["Error"] = $"El número de pedido {entrega.PedidoId} no existe. Verifique e intente nuevamente.";
                return RedirectToAction("Registrar", "Entregas");
            }

            var resultado = entregas.RegistrarEntrega(entrega);

            if (resultado)
            {
                TempData["Mensaje"] = "La entrega se registró correctamente.";
                return RedirectToAction("ListarEntregas");
            }
            else
            {
                TempData["Error"] = "No se pudo registrar la entrega. Intente nuevamente.";
                return View(entrega);
            }
        }


        public ActionResult ListarEntregas()
        {
            var entrega = entregas.ListarEntregas();
            return View(entrega);
        }



        [HttpGet]
        public ActionResult ActualizarEntrega(int id)
        {
            var entrega = entregas.ObtenerEntregaPorId(id);

            if (entrega == null)
            {
                TempData["Error"] = "No se encontró la entrega.";
                return RedirectToAction("ListarEntregas");
            }

            if (!entrega.FechaEntrega.HasValue)
            {
                entrega.FechaEntrega = DateTime.Today;
            }

            return View(entrega);
        }


        [HttpPost]
        public ActionResult ActualizarEntrega(Entrega entrega)
        {
            if (string.IsNullOrWhiteSpace(entrega.CorreoElectronico))
            {
                TempData["Error"] = "El campo de correo electrónico es obligatorio.";
                return View(entrega); 
            }

            var resultado = entregas.ActualizarEntrega(entrega);

            if (resultado)
            {
                TempData["Mensaje"] = "La entrega se actualizó correctamente.";
                return RedirectToAction("ListarEntregas");
            }
            else
            {
                TempData["Error"] = "No se pudo actualizar la entrega. Intente nuevamente.";
                return View(entrega);
            }
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


        [HttpGet]
        public ActionResult CancelarEntrega(int id)
        {

            var entrega = entregas.ObtenerPorId(id);

            if (entrega == null)
            {
                TempData["Error"] = "No se encontró la entrega especificada.";
                return RedirectToAction("ListarEntregas");
            }

            var correoUsuario = entrega.CorreoElectronico; // correo del cliente 
            var correoAdmin = "lthx06@yopmail.com"; //correo del administrador

            var resultado = entregas.CancelarEntrega(id, correoUsuario, correoAdmin);

            if (resultado)
            {
                TempData["Mensaje"] = "La entrega ha sido cancelada y las notificaciones han sido enviadas.";
            }
            else
            {
                TempData["Error"] = "No se pudo cancelar la entrega.";
            }

            return RedirectToAction("ListarEntregas");
        }



    }
}