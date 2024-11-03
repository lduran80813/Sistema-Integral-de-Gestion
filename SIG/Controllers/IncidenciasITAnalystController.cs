using Rotativa;
using SIG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SIG.Controllers
{
    public class IncidenciasITAnalystController : Controller
    {
        CatalogosModel catalogosM = new CatalogosModel();
        IncidenciasModel ticketM = new IncidenciasModel();

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
        public ActionResult ListaTicketsAsignados()
        {
            if (TempData["mensaje"] != null)
            {
                var mensaje = TempData["mensaje"].ToString();
                ViewBag.msj = mensaje;
            }

            var respuesta = ticketM.ListaTicketsAsignados();
            if (respuesta != null) 
                return View(respuesta);
            else
            {
                TempData["mensaje"] = "No se ha podido acceder a la lista de incidencias asignadas";
                return RedirectToAction("Index", "Home");
            }                
        }

        [HttpPost]
        public ActionResult CerrarTicket(Entidades.Ticket ticket)
        {
            var respuesta = ticketM.CerrarTicket(ticket);
            if (respuesta)
            {
                TempData["mensaje"] = "Ticket eliminado correctaqmente";
            }
            else
            {
                TempData["mensaje"] = "No se ha podido eliminar el Ticket";
            }
            return RedirectToAction("ListaTicketsAsignados", "IncidenciasITAnalyst");
        }

        [HttpGet]
        public ActionResult AtenderTicket(int idTicket)
        {
            var respuesta = ticketM.verTicket(idTicket);
            var IdUsuario = int.Parse(Session["IdUsuario"].ToString());

            if (respuesta != null && IdUsuario == respuesta.id_tecnico)
            {
                if (respuesta.estado == 1 || respuesta.estado == 4)
                {
                    TempData["mensaje"] = "Acción denegada: El ticket ya fue cerrado";
                    return RedirectToAction("Index", "Home");
                }

                CatalogosTicket();
                return View(respuesta);
            }
            else
            {
                TempData["mensaje"] = "Acción denegada: No cuenta con permiso para acceder al ticket";
                return RedirectToAction("Index", "Home");
            }
        }


        [HttpPost]
        public ActionResult AtenderTicket(BaseDatos.Ticket ticket)
        {
            var respuesta = ticketM.AtenderTicket(ticket);
            if (respuesta)
            {
                TempData["mensaje"] = "Ticket actualizado exitosamente";
                return RedirectToAction("ListaTicketsAsignados", "IncidenciasITAnalyst");
            }
            else
            {
                ViewBag.msj = "No se ha podido actualizar el Ticket";
                return View();
            }
        }

        [HttpGet]
        public ActionResult ListaHistoricoAtendidos()
        {
            var respuesta = ticketM.ListaHistoricoAtendidos();
            if (respuesta != null)
                return View(respuesta);
            else
            {
                TempData["advertencia"] = "Acción denegada: No cuenta con permiso para acceder al ticket";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public ActionResult ReporteITAnalyst()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ReporteITAnalyst(Entidades.Ticket ticket)
        {
            ticket.id_tecnico = int.Parse(Session["IdUsuario"].ToString());
            Entidades.Ticket respuesta = ticketM.reporteTecnico(ticket);

            if (respuesta != null)
                return View(respuesta);
            //return View("ReporteITAnalystResult", respuesta);

            else
                ViewBag.msj = "No hay datos disponibles para el rango indicado";
            return View();
        }

        [HttpGet]
        public ActionResult ReporteITAnalystPDF(Entidades.Ticket ticket)
        {
            ticket.id_tecnico = int.Parse(Session["IdUsuario"].ToString());
            Entidades.Ticket reporte = ticketM.reporteTecnico(ticket);

            if (reporte != null)
            return new ViewAsPdf("ReporteITAnalystResult", reporte)
            {
                FileName = "ReporteITAnalyst_" + ticket.id_tecnico + "_" + DateTime.Now + ".pdf",
                PageSize = Rotativa.Options.Size.A4,  // Tamaño de página A4
                PageOrientation = Rotativa.Options.Orientation.Portrait,  // Orientación vertical
            };
            return RedirectToAction("ReporteITAnalyst", "IncidenciasITAnalyst");
        }

    }
}