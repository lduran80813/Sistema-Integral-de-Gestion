using SIG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SIG.Controllers
{
    public class NotificacionesController : Controller
    {
        NotificacionesModel notificacionesM = new NotificacionesModel();
        //[HttpGet]
        //public JsonResult ObtenerCantidadNotificaciones()
        //{
        //    var notificaciones = notificacionesM.ConsultarNotificaciones().Count();
        //    return Json(notificaciones, JsonRequestBehavior.AllowGet);
        //}

        [HttpGet]
        public JsonResult ObtenerNotificaciones()
        {
            var notificacionesNuevas = notificacionesM.CantidadNotificacionesNuevas();
            var notificaciones = notificacionesM.ConsultarNotificaciones();

            //List<string> notificaciones
            List<string> lstNotificaciones = new List<string>();
            foreach (var item in notificaciones)
            {
                lstNotificaciones.Add(item.Fecha.ToString() + " " + item.Modulo + "\n" 
                    + item.MensajeB + "\n Id Ticket: " + item.IdReferenciaBD);
            }

            var resultado = new
            {
                cantidad = notificacionesNuevas,
                notificaciones = lstNotificaciones
            };
            return Json(resultado, JsonRequestBehavior.AllowGet);
        }
    }
}