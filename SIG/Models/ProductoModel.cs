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

        public GeneraFacturaEncabezado_Result DatosFacturaEncabezado(int idFactura)
        {
            using (var context = new SistemaIntegralGestionEntities())
            {
                return context.GeneraFacturaEncabezado(idFactura).FirstOrDefault();
            }
        }


        public List<GeneraFacturaDetalle_Result> DatosFacturaDetalle(int idFactura)
        {
            using (var context = new SistemaIntegralGestionEntities())
            {
                return context.GeneraFacturaDetalle(idFactura).ToList();
            }
        }

        public bool ActualizarInventario(InventarioActualizacion ent)
        {
            var rowsAffected = 0;

            using (var context = new SistemaIntegralGestionEntities())
            {
                int Consecutivo = int.Parse(HttpContext.Current.Session["IdUsuario"].ToString());
                rowsAffected = context.actualizaInventario(ent.id, ent.nuevaCantidad, ent.motivo, Consecutivo).Count();
            }
            return (rowsAffected > 0 ? true : false);
        }

        // Aquí voy Hacer el método para extraer los datos de audit productos
        public List<InventarioActualizacion> DatosHistoricos()
        {
            using (var context = new SistemaIntegralGestionEntities())
            {
                return (from h in context.Audit_Venta_Producto
                        join p in context.Venta_Producto on h.id_producto equals p.id
                        join e in context.Empleado on h.responsable equals e.id
                        orderby h.fecha descending
                        select new InventarioActualizacion
                        {
                            fechaAjuste = h.fecha,
                            producto = p.nombre,
                            nuevaCantidad = (int)h.inventario,
                            motivo = h.motivo,
                            nombreResponsable = e.nombre + " " + e.apellidos
                        }).ToList();
            }
        }

    }
}