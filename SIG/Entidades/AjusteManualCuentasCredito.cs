using SIG.BaseDatos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SIG.Entidades
{
    public class AjusteManualCuentasCredito
    {
        public int Id_Cuenta { get; set; }
        public decimal montoPago { get; set; }
        public string descripcion { get; set; }
        public DateTime fechaAjuste { get; set; }
        public string cliente { get; set; }
        public string estado { get; set; }
        public List<SIG.Entidades.CuentasCredito> cuentasCreditos { get; set; }
    }
}