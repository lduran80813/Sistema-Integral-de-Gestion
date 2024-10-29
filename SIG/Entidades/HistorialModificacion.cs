using System;

namespace SIG.Entidades
{
    public class HistorialModificacion
    {
        public int PedidoId { get; set; }
        public DateTime FechaModificacion { get; set; }
        public string DetallesModificacion { get; set; }
    }
}
