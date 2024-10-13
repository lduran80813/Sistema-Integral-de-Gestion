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

        //public void CatalogosTicket()
        //{
        //    // Lista de incidencias
        //    var incidencias = catalogosM.ConsultarTicketTipo();

        //    List<SelectListItem> lstIncidencias = new List<SelectListItem>();
        //    foreach (var item in incidencias)
        //    {
        //        lstIncidencias.Add(new SelectListItem { Value = item.tipo_incidencia.ToString(), Text = item.descripcion.ToString() });
        //    }

        //    ViewBag.Incidencias = lstIncidencias;

        //    // Lista de estados de ticket
        //    var estados = catalogosM.ConsultarEstados();

        //    List<SelectListItem> lstEstados = new List<SelectListItem>();
        //    foreach (var item in estados)
        //    {
        //        lstEstados.Add(new SelectListItem { Value = item.estado_ticket.ToString(), Text = item.descripcion.ToString() });
        //    }

        //    ViewBag.Estados = lstEstados;

        //    // Lista de prioridades de ticket
        //    var prioridades = catalogosM.ConsultarPrioridades();

        //    List<SelectListItem> lstPrioridades = new List<SelectListItem>();
        //    foreach (var item in prioridades)
        //    {
        //        lstPrioridades.Add(new SelectListItem { Value = item.prioridad.ToString(), Text = item.descripcion.ToString() });
        //    }

        //    ViewBag.Prioridades = lstPrioridades;

        //    // Lista de técnicos
        //    var tecnicos = catalogosM.ConsultarTecnicos();

        //    List<SelectListItem> lstTecnicos = new List<SelectListItem>();
        //    foreach (var item in tecnicos)
        //    {
        //        lstTecnicos.Add(new SelectListItem { Value = item.id_usuario.ToString(), Text = item.nombre_completo.ToString() });
        //    }

        //    ViewBag.Tecnicos = lstTecnicos;

        //    // Lista de usuarios
        //    var usuarios = catalogosM.ConsultarUsuarios();

        //    List<SelectListItem> lstUsuarios = new List<SelectListItem>();
        //    foreach (var item in usuarios)
        //    {
        //        lstUsuarios.Add(new SelectListItem { Value = item.id_usuario.ToString(), Text = item.nombre_completo.ToString() });
        //    }

        //    ViewBag.Usuarios = lstUsuarios;
        //}

        //[HttpGet]
        //public ActionResult ListaTicketsRegistrados()
        //{
        //    var respuesta = ticketM.ListaTicketsRegistrados();
        //    return View(respuesta);
        //}


        //[HttpPost]
        //public ActionResult CerrarTicket(Entidades.Ticket ticket)
        //{
        //    var respuesta = ticketM.CerrarTicket(ticket);
        //    if (respuesta)
        //    {
        //        ViewBag.msj = "Ticket eliminado correctaqmente";
        //        return RedirectToAction("ListaTicketsRegistrados", "IncidenciasITManager");
        //    }
        //    else
        //    {
        //        ViewBag.msj = "No se ha podido eliminar el Ticket";
        //        return View(ListaTicketsRegistrados());
        //    }
        //}

        //[HttpGet]
        //public ActionResult AsignarTicket(int idTicket)
        //{
        //    var respuesta = ticketM.verTicket(idTicket);

        //    if (respuesta.estado != 1)
        //    {
        //        TempData["advertencia"] = "Acción denegada: El ticket ya fue asignado y priorizado";
        //        return RedirectToAction("Index", "Home");
        //    }

        //    CatalogosTicket();
        //    return View(respuesta);
        //}


        //[HttpPost]
        //public ActionResult AsignarTicket(BaseDatos.Ticket ticket)
        //{
        //    var respuesta = ticketM.AsignarTicket(ticket);
        //    if (respuesta)
        //    {
        //        TempData["advertencia"] = "Ticket asignado existosamente";                
        //    }                
        //    else
        //    {
        //        TempData["advertencia"] = "No se ha podido actualizar el ticket";
        //    }
        //    return RedirectToAction("ListaTicketsRegistrados", "IncidenciasITManager");
        //}

        //[HttpGet]
        //public ActionResult ListaHistoricoTickets()
        //{
        //    var respuesta = ticketM.ListaHistoricoTickets();
        //    return View(respuesta);
        //}

    }
}