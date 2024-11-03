using System;

namespace SIG.Entidades
{
    public class HistorialPedido
    {
        public int PedidoId { get; set; }
        public DateTime FechaModificacion { get; set; } 
        public string DetalleCambio { get; set; } 
        public int EstadoCompraId { get; set; } 
    }
}
