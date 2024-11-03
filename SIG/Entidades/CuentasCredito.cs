using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SIG.Entidades
{
    public class CuentasCredito
    {
        public int Id_Cuenta{ get; set; }
        public int IdReferencia { get; set; }
        public decimal SaldoAnterior { get; set; }
        public decimal Abono { get; set; }
        public decimal SaldoActual { get; set; }
        public List<SIG.BaseDatos.ListarCxC_Result> ListaCxC { get; set; }
        public List<SIG.BaseDatos.ListarCxP_Result> ListaCxP { get; set; }
        public int metodo_pago { get; set; }
        public string entidadFinanciera { get; set; }
        public decimal transaccionRef { get; set; }
        public decimal montoPago { get; set; }
        public string descripcion { get; set; }
    }
}