using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SIG.Entidades
{
    public class Entrega
    {
        public int? Id { get; set; }
        public int? PedidoId { get; set; }
        public DateTime? FechaEntrega { get; set; } = DateTime.Today;
        public string DireccionEntrega { get; set; }
        public string ArticulosEntregados { get; set; }
        public string ObservacionesAdicionales { get; set; }

        public string EstadoEntrega { get; set; }  
        public string NombreDestinatario { get; set; }

        public string CorreoElectronico { get; set; }

        public int ClienteId { get; set; }
    }


}