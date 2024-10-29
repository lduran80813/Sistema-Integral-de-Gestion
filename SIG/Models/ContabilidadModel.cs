using SIG.BaseDatos;
using SIG.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SIG.Models
{
    public class ContabilidadModel
    {
        public List<ListarCxC_Result> ListaCxC()
        {
            using (var context = new SistemaIntegralGestionEntities())
            {
                return context.ListarCxC().ToList();
            }
        }

        public bool ContaPagoCxC(CuentasCredito ent)
        {
            var rowsAffected = 0;

            using (var context = new SistemaIntegralGestionEntities())
            {
                int Consecutivo = int.Parse(HttpContext.Current.Session["IdUsuario"].ToString());
                rowsAffected = context.PagarCxC(ent.Id_Cuenta, ent.montoPago, ent.metodo_pago, ent.entidadFinanciera, ent.transaccionRef, ent.descripcion, Consecutivo);
            }
            return (rowsAffected > 0 ? true : false);
        }
        public List<ListarCxP_Result> ListaCxP()
        {
            using (var context = new SistemaIntegralGestionEntities())
            {
                return context.ListarCxP().ToList();
            }
        }
        public bool ContaPagoCxP(CuentasCredito ent)
        {
            var rowsAffected = 0;

            using (var context = new SistemaIntegralGestionEntities())
            {
                int Consecutivo = int.Parse(HttpContext.Current.Session["IdUsuario"].ToString());
                rowsAffected = context.PagarCxP(ent.Id_Cuenta, ent.montoPago, ent.metodo_pago, ent.entidadFinanciera, ent.transaccionRef, ent.descripcion, Consecutivo);
            }
            return (rowsAffected > 0 ? true : false);
        }

        public List<cierre_contable_Result> CierreContable(RangoFecha fecha)
        {
            using (var context = new SistemaIntegralGestionEntities())
            {
                return context.cierre_contable(fecha.inicioCorte, fecha.finCorte).ToList();
            }
        }

        public bool RegistroTransaccionFinanciera(TransaccionFinanciera ent)
        {
            var rowsAffected = 0;

            using (var context = new SistemaIntegralGestionEntities())
            {
                int Consecutivo = int.Parse(HttpContext.Current.Session["IdUsuario"].ToString());
                rowsAffected = context.RegistrarTransaccionesFinancieras(ent.IdCuenta, ent.Monto, ent.Descripcion, Consecutivo);
            }
            return (rowsAffected > 0 ? true : false);
        }
    }
}