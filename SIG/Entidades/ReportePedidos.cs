using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SIG.Entidades
{
    public class ReportePedidos
    {
        public int PedidoId { get; set; }
        public DateTime FechaPedido { get; set; }
        public string Proveedor { get; set; }
        public decimal Total { get; set; }
        public string Evaluacion { get; set; } 
    }
}
