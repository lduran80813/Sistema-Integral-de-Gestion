using Rotativa;
using SIG.BaseDatos;
using SIG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SIG.Controllers
{
    public class IncidenciasITManagerController : Controller
    {
        CatalogosModel catalogosM = new CatalogosModel();
        IncidenciasModel ticketM = new IncidenciasModel();
        NotificacionesModel notificacionesM = new NotificacionesModel();

        public void CatalogosTicket()
        {
            // Lista de incidencias
            var incidencias = catalogosM.ConsultarTicketTipo();

            List<SelectListItem> lstIncidencias = new List<SelectListItem>();
            foreach (var item in incidencias)
            {
                lstIncidencias.Add(new SelectListItem { Value = item.tipo_incidencia.ToString(), Text = item.descripcion.ToString() });
            }

            ViewBag.Incidencias = lstIncidencias;

            // Lista de estados de ticket
            var estados = catalogosM.ConsultarEstados();

            List<SelectListItem> lstEstados = new List<SelectListItem>();
            foreach (var item in estados)
            {
                lstEstados.Add(new SelectListItem { Value = item.estado_ticket.ToString(), Text = item.descripcion.ToString() });
            }

            ViewBag.Estados = lstEstados;

            // Lista de prioridades de ticket
            var prioridades = catalogosM.ConsultarPrioridades();

            List<SelectListItem> lstPrioridades = new List<SelectListItem>();
            foreach (var item in prioridades)
            {
                lstPrioridades.Add(new SelectListItem { Value = item.prioridad.ToString(), Text = item.descripcion.ToString() });
            }

            ViewBag.Prioridades = lstPrioridades;

            // Lista de técnicos
            var tecnicos = catalogosM.ConsultarTecnicos();

            List<SelectListItem> lstTecnicos = new List<SelectListItem>();
            foreach (var item in tecnicos)
            {
                lstTecnicos.Add(new SelectListItem { Value = item.id_usuario.ToString(), Text = item.nombre_completo.ToString() });
            }

            ViewBag.Tecnicos = lstTecnicos;

            // Lista de usuarios
            var usuarios = catalogosM.ConsultarUsuarios();

            List<SelectListItem> lstUsuarios = new List<SelectListItem>();
            foreach (var item in usuarios)
            {
                lstUsuarios.Add(new SelectListItem { Value = item.id_usuario.ToString(), Text = item.nombre_completo.ToString() });
            }

            ViewBag.Usuarios = lstUsuarios;
        }

        [HttpGet]
        public ActionResult ListaTicketsRegistrados()
        {
            if (TempData["mensaje"] != null)
            {
                var mensaje = TempData["mensaje"].ToString();
                ViewBag.msj = mensaje;
            }
            var respuesta = ticketM.ListaTicketsRegistrados();
            return View(respuesta);
        }


        [HttpPost]
        public ActionResult CerrarTicket(Entidades.Ticket ticket)
        {
            var respuesta = ticketM.CerrarTicket(ticket);
            if (respuesta)
            {
                TempData["mensaje"] = "Ticket eliminado correctaqmente";
                return RedirectToAction("ListaTicketsRegistrados", "IncidenciasITManager");
            }
            else
            {
                TempData["mensaje"] = "No se ha podido eliminar el Ticket";
                return RedirectToAction("ListaTicketsRegistrados", "IncidenciasITManager");
            }
        }

        [HttpGet]
        public ActionResult AsignarTicket(int idTicket)
        {
            var respuesta = ticketM.verTicket(idTicket);

            if (respuesta.estado != 1)
            {
                TempData["mensaje"] = "Acción denegada: El ticket ya fue asignado y priorizado";
                return RedirectToAction("Index", "Home");
            }

            CatalogosTicket();
            return View(respuesta);
        }


        [HttpPost]
        public ActionResult AsignarTicket(BaseDatos.Ticket ticket)
        {
            var respuesta = ticketM.AsignarTicket(ticket);

            // Notificacion
            notificacionesM.NuevaNotificacion(2, 2, 1, ticket.id_ticket, (int) ticket.id_tecnico); //Módulo 2, Notif básica 2, prioridad 1, idEnviar id_tecnico del Ticket



            if (respuesta)
            {
                TempData["mensaje"] = "Ticket asignado existosamente";                
            }                
            else
            {
                TempData["mensaje"] = "No se ha podido actualizar el ticket";
            }
            return RedirectToAction("ListaTicketsRegistrados", "IncidenciasITManager");
        }

        [HttpGet]
        public ActionResult ListaHistoricoTickets()
        {
            var respuesta = ticketM.ListaHistoricoTickets();
            if (respuesta != null)
                return View(respuesta);
            else
            {
                TempData["mensaje"] = "Acción denegada: No cuenta con permiso para acceder al ticket";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public ActionResult ListaTiposTicket()
        {
            var respuesta = catalogosM.ConsultarTicketTipo();

            if (TempData["mensaje"] != null)
            {
                var mensaje = TempData["mensaje"].ToString();
                ViewBag.msj = mensaje;
            }

            if (respuesta != null)
                return View(respuesta);
            else
            {
                TempData["mensaje"] = "Acción denegada: No cuenta con permiso para acceder a los tipos de incidencias";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public ActionResult DesactivarTipoIncidencia(BaseDatos.Ticket_Tipo tipo)
        {
            var respuesta = ticketM.DesactivarTipoIncidencia(tipo);
            if (respuesta)
            {
                TempData["mensaje"] = "Tipo de incidencia desactivada correctamente";
                return RedirectToAction("ListaTiposTicket", "IncidenciasITManager");
            }
            else
            {
                TempData["mensaje"] = "No se ha podido desactivar el tipo de incidencia";
                return RedirectToAction("ListaTiposTicket", "IncidenciasITManager");
            }
        }

        [HttpGet]
        public ActionResult CrearCategoria()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CrearCategoria(Entidades.CategoriaIncidencia categoria)
        {
            var respuesta = ticketM.CrearCategoria(categoria);

            if (respuesta)
            {
                TempData["mensaje"] = "Categoría creada exitosamente";
                return RedirectToAction("ListaTiposTicket", "IncidenciasITManager");
            }
            else
            {
                ViewBag.msj = "No se ha podido crear categoría";
                return View();
            }
        }

        [HttpGet]
        public ActionResult EditarCategoria(int idCategoria)
        {
            var respuesta = ticketM.verCategoria(idCategoria);


            if (respuesta != null)
            {
                return View(respuesta);
            }
            else
            {
                TempData["mensaje"] = "Acción denegada: No cuenta con permiso para acceder a los tipos de incidencias";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public ActionResult EditarCategoria(BaseDatos.Ticket_Tipo categoria)
        {
            var respuesta = ticketM.ActualizarCategoria(categoria);
            if (respuesta)
            {
                TempData["mensaje"] = "Categoría actualizada exitosamente";
                return RedirectToAction("ListaTiposTicket", "IncidenciasITManager");
            }
            else
            {
                ViewBag.msj = "No se ha podido actualizar la categoría";
                return View();
            }
        }

        public void ListaTecnicos()
        {
            // Lista de técnicos
            var tecnicos = catalogosM.ConsultarTecnicos();

            List<SelectListItem> lstTecnicos = new List<SelectListItem>();
            lstTecnicos.Add(new SelectListItem { Value = "0", Text = "Todos (grupal)" });
            foreach (var item in tecnicos)
            {
                lstTecnicos.Add(new SelectListItem { Value = item.id_usuario.ToString(), Text = item.nombre_completo.ToString() });
            }

            ViewBag.Tecnicos = lstTecnicos;
        }

        [HttpGet]
        public ActionResult ReporteITManager()
        {
            ListaTecnicos();
            return View();
        }


        [HttpPost]
        public ActionResult ReporteITManager(Entidades.Ticket ticket)
        {
            Entidades.Ticket respuesta = new Entidades.Ticket();
            if (ticket.id_tecnico != 0)
                respuesta = ticketM.reporteTecnico(ticket);
            else respuesta = ticketM.reporteTecnicos(ticket);


            if (respuesta != null)
            {
                ListaTecnicos();
                return View(respuesta);
            }
            //return View("ReporteITManagerResult", respuesta);

            else {
                ViewBag.msj = "No hay datos disponibles para el rango indicado";
                ListaTecnicos();
                return View();
            }
                
        }

        [HttpGet]
        public ActionResult ReporteITManagerPDF(Entidades.Ticket ticket)
        {
            Entidades.Ticket reporte= new Entidades.Ticket();
            if (ticket.id_tecnico != 0)
                reporte = ticketM.reporteTecnico(ticket);
            else reporte = ticketM.reporteTecnicos(ticket);

            if (reporte != null)
                return new ViewAsPdf("ReporteITManagerResult", reporte)
                {
                    FileName = "ReporteITManager_" + ticket.id_tecnico + "_" + DateTime.Now + ".pdf",
                    PageSize = Rotativa.Options.Size.A4,  // Tamaño de página A4
                    PageOrientation = Rotativa.Options.Orientation.Portrait,  // Orientación vertical
                };
            return RedirectToAction("ReporteITManager", "IncidenciasITManager");
        }
    }
}