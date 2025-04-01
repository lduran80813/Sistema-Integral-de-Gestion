using System;

namespace SIG.Entidades
{
    public class Pedido
    {
        public int id { get; set; }
        public DateTime fecha_pedido { get; set; }
        public int proveedor_id { get; set; }
        public int total_pedido { get; set; }
        public int EstadoCompraId { get; set; }
        public int TipoCompraId { get; set; }
    }
}
