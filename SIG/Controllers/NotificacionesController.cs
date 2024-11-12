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
            //List<string> lstNotificaciones = new List<string>();
            foreach (var item in notificaciones)
            {
                //lstNotificaciones.Add(item.Fecha.ToString() + " " + item.Modulo + "\n" 
                //    + item.MensajeB + "\n Id de elemento: " + item.IdReferenciaBD);
                item.MensajeF = item.Fecha.ToString() + " " + item.Modulo + "\n"
                    + item.MensajeB + "\n Id de elemento: " + item.IdReferenciaBD;
            }

            var resultado = new
            {
                cantidad = notificacionesNuevas,
                notificaciones = notificaciones
            };
            return Json(resultado, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult CambiarEstadoLectura(int idNotificacion)
        {
            bool resultado = notificacionesM.CambiarEstadoLectura(idNotificacion);
            return Json(new { success = resultado });
        }
    }
}