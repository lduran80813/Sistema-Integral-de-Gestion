using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SIG.Entidades
{
    public class Entrega
    {
        public int? Id { get; set; }
        public int PedidoId { get; set; }
        public DateTime FechaEntrega { get; set; }
        public string DireccionEntrega { get; set; }
        public string ArticulosEntregados { get; set; }
        public string ObservacionesAdicionales { get; set; }
    }

}