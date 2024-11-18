using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SIG.Entidades
{
    public class ReportePlan
    {
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public int TotalPlanes { get; set; }
        public int PlanesFinalizados { get; set; }
        public int PlanesPendientes { get; set; }
    }
}