using SIG.BaseDatos;
using SIG.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SIG.Models
{
    public class ProductoModel
    {
        public List<Producto> ConsultarProductos()
        {
            using (var context = new SistemaIntegralGestionEntities())
            {
                return (from x in context.Venta_Producto
                        join e in context.Venta_ProductoEstado on x.estado equals e.id
                        //where e.descripcion == "Existencias"
                        orderby x.estado descending
                        select new Producto
                        {
                            id = x.id,
                            nombre = x.nombre,
                            descripcion = x.descripcion,
                            precio = x.precio,
                            estado = x.estado,
                            estadoDescripcion = e.descripcion,
                            inventario = x.inventario}).ToList();
            }
        }

        public bool RegistrarCarrito(int idProducto, int Cantidad)
        {
            var rowsAffected = 0;


            using (var context = new SistemaIntegralGestionEntities())
            {
                int ConsecutivoSesion = int.Parse(HttpContext.Current.Session["IdUsuario"].ToString());
                rowsAffected = context.RegistrarCarrito(ConsecutivoSesion, idProducto, Cantidad);
            }

            return (rowsAffected > 0 ? true : false);
        }

        public List<ConsultarCarrito_Result> ConsultarCarrito()
        {
            using (var context = new SistemaIntegralGestionEntities())
            {
                int Consecutivo = int.Parse(HttpContext.Current.Session["IdUsuario"].ToString());
                return context.ConsultarCarrito(Consecutivo).ToList();
            }
        }

        public List<ValidarExistencias_Result> ValidarExistencias()
        {
            using (var context = new SistemaIntegralGestionEntities())
            {
                int Consecutivo = int.Parse(HttpContext.Current.Session["IdUsuario"].ToString());
                return context.ValidarExistencias(Consecutivo).ToList();
            }
        }

        public bool EliminarProductoPedido(int idProducto)
        {
            var rowsAffected = 0;

            using (var context = new SistemaIntegralGestionEntities())
            {
                int Consecutivo = int.Parse(HttpContext.Current.Session["IdUsuario"].ToString());
                rowsAffected = context.EliminarProductoCarrito(Consecutivo, idProducto);
            }
            return (rowsAffected > 0 ? true : false);
        }

        public bool ProcesarCarrito(int idCliente, int estado, string notasAdicionales)
        {
            var rowsAffected = 0;

            using (var context = new SistemaIntegralGestionEntities())
            {
                // El cliente debe incluirse en formulario antes de procesar estos datos, está pendiente

                int Consecutivo = int.Parse(HttpContext.Current.Session["IdUsuario"].ToString());
                rowsAffected = context.ProcesarCarrito(idCliente, Consecutivo, estado, notasAdicionales);
            }
            return (rowsAffected > 0 ? true : false);
        }

        public List<ConsultarPedidos_Result> ConsultarPedidos()
        {
            using (var context = new SistemaIntegralGestionEntities())
            {
                return context.ConsultarPedidos().ToList();
            }
        }
        public List<ConsultarDetallePedido_Result> ConsultarDetallePedido(int idPedido)
        {
            using (var context = new SistemaIntegralGestionEntities())
            {
                return context.ConsultarDetallePedido(idPedido).ToList();
            }
        }

        public bool FacturarPedido(PedidoFactura ent)
        {
            var rowsAffected = 0;

            using (var context = new SistemaIntegralGestionEntities())
            {
                rowsAffected = context.FacturarPedido(ent.idFacturar, ent.tipo_venta);
            }
            return (rowsAffected > 0 ? true : false);
        }

        public bool ContaPagoFactura(PedidoFactura ent)
        {
            var rowsAffected = 0;

            using (var context = new SistemaIntegralGestionEntities())
            {
                int Consecutivo = int.Parse(HttpContext.Current.Session["IdUsuario"].ToString());
                rowsAffected = context.PagoPedido(ent.idPagar, ent.tipo_venta, ent.metodo_pago, ent.entidadFinanciera, ent.transaccionRef, ent.montoPago, ent.descripcion, Consecutivo);
            }
            return (rowsAffected > 0 ? true : false);
        }

        public datosRecibo_Result DatosRecibo(int idPedido)
        {
            using (var context = new SistemaIntegralGestionEntities())
            {
                return context.datosRecibo(idPedido).FirstOrDefault();
            }
        }

    }
}