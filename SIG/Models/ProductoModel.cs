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

    }
}