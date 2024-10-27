using SIG.Entidades;
using SIG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SIG.Controllers
{
    public class ContabilidadController : Controller
    {
        ProductoModel productosM = new ProductoModel();

        private void CargarVariablesCarrito()
        {
            var carritoActual = productosM.ConsultarCarrito();
            Session["Cantidad"] = carritoActual.Sum(c => c.Cantidad);
            Session["SubTotal"] = carritoActual.Sum(c => c.SubTotal);
            Session["Total"] = carritoActual.Sum(c => c.Total);
        }

        [HttpGet]
        public ActionResult Productos_Ventas()
        {
            CargarVariablesCarrito();

            var respuesta = productosM.ConsultarProductos();
            return View(respuesta);
        }

        [HttpPost]
        public ActionResult Productos_Ventas(int IdProducto, int Cantidad)
        {
            productosM.RegistrarCarrito(IdProducto, Cantidad);

            CargarVariablesCarrito();

            return Json("", JsonRequestBehavior.AllowGet);
        }

        public ActionResult RegistrarCarrito(int IdProducto, int Cantidad)
        {
            productosM.RegistrarCarrito(IdProducto, Cantidad);

            CargarVariablesCarrito();

            return Json("", JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult ConsultarPedido()
        {
            var datos = productosM.ConsultarCarrito();

            // Se setea el objeto general que tiene el id del producto necesario del modal y la lista actual del carrito
            Carrito carrito = new Carrito();
            carrito.DatosCarrito = datos;

            Session["Cantidad"] = datos.Sum(c => c.Cantidad);
            Session["SubTotal"] = datos.Sum(c => c.SubTotal);
            Session["Total"] = datos.Sum(c => c.Total);

            return View(carrito);
        }

        [HttpPost]
        public ActionResult EliminarProductoPedido(Carrito ent)
        {
            productosM.EliminarProductoPedido(ent.IdProducto);
            return RedirectToAction("ConsultarPedido", "Contabilidad");
        }

        public ActionResult Generar_Factura()
        {
            return View();
        }

        public ActionResult CxP()
        {
            return View();
        }
    }
}