using SIG.BaseDatos;
using SIG.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static System.ActivationContext;

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

        public InformeVentas Informe_Ventas(InformeVentas fecha)
        {
            using (var context = new SistemaIntegralGestionEntities())
            {
                fecha.datosInforme = context.Informe_Ventas(fecha.inicioCorte, fecha.finCorte).ToList();

                fecha.cantidadVentas = (from x in context.Venta_Factura
                                     where x.fecha_factura >= fecha.inicioCorte && x.fecha_factura <= fecha.finCorte && x.estado > 2
                                     select x).Count();

                fecha.ingresosTotales = (decimal)(from x in context.Venta_Factura
                                      where x.fecha_factura >= fecha.inicioCorte && x.fecha_factura <= fecha.finCorte && x.estado > 2
                                      select x.total_transaccion).Sum();

                fecha.principalesClientes = context.top5Clientes(fecha.inicioCorte, fecha.finCorte).ToList();

                fecha.distribucionMediosPago = context.distribMetodoPago(fecha.inicioCorte, fecha.finCorte).ToList();

                return (fecha);
            }
        }
    }
}