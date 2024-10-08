using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SIG.Entidades
{
    public class EstadoFinanciero
    {
        public int ProveedorId { get; set; }
        public string NombreProveedor { get; set; }
        public decimal Saldo { get; set; }
        public string Estado { get; set; } 
        public string MensajeError { get; set; } 
    }
}