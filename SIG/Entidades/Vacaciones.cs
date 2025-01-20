using SIG.BaseDatos;
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
        public string EmpleadoNombre { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public string Estado { get; set; }
        public int DiasSolicitados { get; set; }
        public DateTime FechaSolicitud { get; set; }
        public string Observaciones { get; set; }
        public string MotivoRechazo { get; set; }
        public int? AprobadoPor { get; set; }
        public string AdministradorNombre { get; set; }
        public DateTime? FechaAprobacion { get; set; }


    }
}