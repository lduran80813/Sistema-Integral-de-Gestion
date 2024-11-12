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

        public bool CambiarEstadoLectura(int idNotificacion)
        {
            var rowsAffected = 0;
            using (var context = new SistemaIntegralGestionEntities())
            {
                rowsAffected = context.CambiarEstadoLectura(idNotificacion);
                return (rowsAffected > 0 ? true : false);
            }
        }

        public bool NuevaNotificacion(int idModulo, int idNotificacionBasica, int? prioridad, int idReferencia,
            int idUsuarioReceptor)
        {
            using (var context = new SistemaIntegralGestionEntities())
            {
                int idUsuario = int.Parse(HttpContext.Current.Session["IdUsuario"].ToString());
                var rowsAffected = context.NuevaNotificacion(idUsuario, idModulo, idNotificacionBasica, prioridad, idReferencia, idUsuarioReceptor).Count();
                return (rowsAffected > 0 ? true : false);
            }
        }
    }
}