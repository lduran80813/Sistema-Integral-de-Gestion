using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SIG.Entidades
{
    public class FacturaPDF
    {
        public SIG.BaseDatos.GeneraFacturaEncabezado_Result Encabezado { get; set; }
        public List<SIG.BaseDatos.GeneraFacturaDetalle_Result> Detalle { get; set; }
    }
}