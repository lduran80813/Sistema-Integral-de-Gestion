using SIG.BaseDatos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SIG.Entidades
{
    public class PedidoFactura
    {
        public int idFacturar { get; set; }
        public int idPagar { get; set; }
        public int tipo_venta { get; set; }
        public int metodo_pago { get; set; }
        public List<SIG.BaseDatos.ConsultarPedidos_Result> Pedidos { get; set; }
        public string entidadFinanciera { get; set; }
        public string descripcion { get; set; }
        public decimal transaccionRef { get; set; }
        public decimal montoPago { get; set; }
    }
}