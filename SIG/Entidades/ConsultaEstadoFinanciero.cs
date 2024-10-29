using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SIG.Entidades
{
    public class ConsultaEstadoFinanciero
    {
        public List<int> ProveedorIds { get; set; } // Para manejar múltiples IDs de proveedores
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
    }
}