using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SIG.Entidades
{
    public class TransaccionFinanciera
    {
        public int IdCuenta { get; set; }
        public decimal Monto { get; set; }
        public string Descripcion { get; set; }
    }
}