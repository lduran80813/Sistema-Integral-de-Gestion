using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SIG.Entidades
{
    public class Notificacion
    {
        public int PedidoId { get; set; }
        public DateTime? FechaEntrega { get; set; }
        public string DireccionEntrega { get; set; }
        public string NombreDestinatario { get; set; }
        public string EstadoEntrega { get; set; }
    }
}