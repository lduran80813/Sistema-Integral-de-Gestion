using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SIG.Entidades
{
    public class SolicitudVacacionViewModel
    {
        public int SolicitudId { get; set; }
        public string EmpleadoNombre { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public int DiasSolicitados { get; set; }
        public string Estado { get; set; }
        public string Observaciones { get; set; }
    }
}