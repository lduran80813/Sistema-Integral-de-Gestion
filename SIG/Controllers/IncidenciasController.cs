using SIG.BaseDatos;
using SIG.Entidades;
using SIG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SIG.Controllers
{
    public class IncidenciasController : Controller
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

        // GET: Incidencias
        [HttpGet]
        public ActionResult NuevoTicket()
        {
            var tipo = catalogosM.ConsultarTicketTipo();

            List<SelectListItem> lstIncidencias = new List<SelectListItem>();
            foreach (var item in tipo)
            {
                lstIncidencias.Add(new SelectListItem { Value = item.tipo_incidencia.ToString(), Text = item.descripcion.ToString() });
            }

            ViewBag.Incidencias = lstIncidencias;


            return View();
        }

        [HttpPost]
        public ActionResult NuevoTicket(Entidades.Ticket ticket) 
        {
            var respuesta = ticketM.InsertarTicket(ticket);

            if (respuesta)
            {
                TempData["mensaje"] = "Incidencia creada exitosamente";
                return RedirectToAction("ListaIncidenciasUsuario", "Incidencias");
            }                
            else
            {
                ViewBag.msj = "No se ha podido crear el ticket";

                var tipo = catalogosM.ConsultarTicketTipo();

                List<SelectListItem> lstIncidencias = new List<SelectListItem>();
                foreach (var item in tipo)
                {
                    lstIncidencias.Add(new SelectListItem { Value = item.tipo_incidencia.ToString(), Text = item.descripcion.ToString() });
                }

                ViewBag.Incidencias = lstIncidencias;
                return View();
            }

        }

        [HttpGet]
        public ActionResult ListaIncidenciasUsuario()
        {
            var respuesta = ticketM.ListaIncidenciasUsuario();
            if (respuesta == null) 
                ViewBag.msj = "No se ha podido conectar con la base de datos";

            if (TempData["mensaje"] != null)
            {
                var mensaje = TempData["mensaje"].ToString();
                ViewBag.msj = mensaje;
            }

            return View(respuesta);
        }

        
        [HttpGet]
        public ActionResult ActualizarTicket(int idTicket)
        {
            var respuesta = ticketM.verTicket(idTicket);
            var IdUsuario = int.Parse(Session["IdUsuario"].ToString());


            if (respuesta != null && IdUsuario == respuesta.id_usuario)
            {
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
        public ActionResult ActualizarTicket(BaseDatos.Ticket ticket)
        {
            var respuesta = ticketM.ActualizarTicket(ticket);
            if (respuesta)
            {
                TempData["mensaje"] = "Ticket actualizado exitosamente";
                return RedirectToAction("ListaIncidenciasUsuario", "Incidencias");
            }
            else
            {
                ViewBag.msj = "No se ha podido actualizar el ticket";
                CatalogosTicket();
                return View();
            }
        }

        [HttpPost]
        public ActionResult CerrarTicket(Entidades.Ticket ticket)
        {
            var respuesta = ticketM.CerrarTicket(ticket);
            if (respuesta)
            {
                TempData["mensaje"] = "Ticket eliminado correctaqmente";
                return RedirectToAction("ListaIncidenciasUsuario", "Incidencias");
            }                
            else
            {
                TempData["mensaje"] = "No se ha podido eliminar el Ticket";
                return RedirectToAction("ListaIncidenciasUsuario", "Incidencias");
            }
        }


        [HttpGet]
        public ActionResult ListaHistoricoUsuario()
        {
            var respuesta = ticketM.ListaHistoricoUsuario();
            return View(respuesta);
        }

        [HttpGet]
        public ActionResult VerTicket(int idTicket, string accion, string controlador)
        {
            var respuesta = ticketM.verTicket(idTicket);
            var rolUsuario = int.Parse(Session["RolUsuario"].ToString());
            var idUsuario = int.Parse(Session["IdUsuario"].ToString());

            if (respuesta != null && (rolUsuario == 3 || idUsuario == respuesta.id_usuario || idUsuario == respuesta.id_tecnico))
            {
                CatalogosTicket();
                ViewBag.Accion = accion;
                ViewBag.Controlador = controlador;
                return View(respuesta);
            }
            else
            {
                TempData["mensaje"] = "Acción denegada: No cuenta con permiso para acceder al ticket";
                return RedirectToAction("Index", "Home");
            }
        }

    }

}