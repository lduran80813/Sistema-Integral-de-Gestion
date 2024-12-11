using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SIG.Entidades
{
    public class Vacaciones
    {
        public int SolicitudId { get; set; }
        public int EmpleadoId { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public string Estado { get; set; }
        public string Comentario { get; set; }
        public int SupervisorId { get; set; }
        public DateTime FechaSolicitud { get; set; }
        public DateTime? FechaRespuesta { get; set; }
    }
}