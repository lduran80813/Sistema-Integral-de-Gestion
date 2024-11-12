using SIG.BaseDatos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SIG.Models
{
    public class NotificacionesModel
    {
        public int CantidadNotificacionesNuevas()
        {
            using (var context = new SistemaIntegralGestionEntities())
            {
                int idUsuario = int.Parse(HttpContext.Current.Session["IdUsuario"].ToString());
                return (int)context.CantidadNotificacionesNuevas(idUsuario).FirstOrDefault();
            }
        }

        public List<ObtenerNotificaciones_Result> ConsultarNotificaciones()
        {
            using (var context = new SistemaIntegralGestionEntities())
            {
                int idUsuario = int.Parse(HttpContext.Current.Session["IdUsuario"].ToString());
                return context.ObtenerNotificaciones(idUsuario).ToList();
            }
        }
    }
}