using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SIG.Entidades
{
    public class ProveedorFinanciero
    {
        public int ProveedorId { get; set; }
        public string NombreProveedor { get; set; }
        public decimal TotalComprasContado { get; set; }
        public decimal TotalComprasCredito { get; set; }
        public decimal TotalCompras { get; set; }
        public DateTime FechaCorte { get; set; }
    }
}