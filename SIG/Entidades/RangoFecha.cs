using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SIG.Entidades
{
    public class RangoFecha
    {
        public System.DateTime inicioCorte { get; set; }
        public System.DateTime finCorte { get; set; }
        public List<SIG.BaseDatos.cierre_contable_Result> cierreContable { get; set; }
    }
}