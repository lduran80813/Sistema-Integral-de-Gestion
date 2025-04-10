﻿using DocumentFormat.OpenXml.Drawing.Diagrams;
using Rotativa;
using SIG.BaseDatos;
using SIG.Entidades;
using SIG.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Sockets;
using System.Web;
using System.Web.Mvc;

namespace SIG.Controllers
{
    public class ContabilidadController : Controller
    {
        ProductoModel productosM = new ProductoModel();
        CatalogosModel catalogosM = new CatalogosModel();
        ContabilidadModel contabilidadM = new ContabilidadModel();
        NotificacionesModel notificacionesM = new NotificacionesModel();

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
        //public void ListaClientes()
        //{
        //    // Lista de usuarios
        //    var usuarios = catalogosM.ConsultarUsuarios();

        //    List<SelectListItem> lstUsuarios = new List<SelectListItem>();
        //    foreach (var item in usuarios)
        //    {
        //        lstUsuarios.Add(new SelectListItem { Value = item.id_usuario.ToString(), Text = item.nombre_completo.ToString() });
        //    }

        //    ViewBag.Usuarios = lstUsuarios;
        //}
        [HttpGet]
        public ActionResult ConsultarPedido()
        {
            var datos = productosM.ConsultarCarrito();

            // Se setea el objeto general que tiene el id del producto necesario del modal y la lista actual del carrito
            Carrito carrito = new Carrito();
            carrito.DatosCarrito = datos;
            carrito.Clientes = catalogosM.ConsultarClientes();

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

        [HttpPost]
        public ActionResult ProcesarPedido(int idCliente, string notasAdicionales)
        {
            var datos = productosM.ValidarExistencias();

            // No hay existencia en incumplimiento
            if (datos.Count() <= 0)
            {
                productosM.ProcesarCarrito(idCliente, 1, notasAdicionales);
                return RedirectToAction("ListaPedidos", "Contabilidad");
            }
            else
            {
                var productosEnCarrito = productosM.ConsultarCarrito();

                // Se setea el objeto general que tiene el id del producto necesario del modal y la lista actual del carrito
                Carrito carrito = new Carrito();
                carrito.DatosCarrito = productosEnCarrito;

                var mensaje = "";
                var contador = 0;
                foreach (var item in datos)
                {
                    if (contador == datos.Count())
                    {
                        mensaje += "y " + item.descripcion;
                    }
                    else
                    {
                        mensaje += item.descripcion + ", ";
                        contador += 1;
                    }
                }

                ViewBag.msj = "No se puede realizar el pago, los productos " + mensaje + " superan la cantidad en el inventario disponible";
                return View("ConsultarPedido", carrito);
            }

        }

        [HttpGet]
        public ActionResult ListaPedidos()
        {
            // Tipo Venta
            var tipo = catalogosM.ConsultarTipoVenta();

            List<SelectListItem> lstTipoVentas = new List<SelectListItem>();
            foreach (var item in tipo)
            {
                lstTipoVentas.Add(new SelectListItem { Value = item.id.ToString(), Text = item.descripcion.ToString() });
            }

            ViewBag.TipoVentas = lstTipoVentas;

            // Método pago
            var metodoPago = catalogosM.ConsultarMetodoPago();

            List<SelectListItem> lstMetodoPago = new List<SelectListItem>();
            foreach (var item in metodoPago)
            {
                lstMetodoPago.Add(new SelectListItem { Value = item.id.ToString(), Text = item.metodo.ToString() });
            }

            ViewBag.MetodoPago = lstMetodoPago;

            CatalogosTransaccionesFinancieras();

            PedidoFactura pf = new PedidoFactura();
            pf.Pedidos = productosM.ConsultarPedidos();
            return View(pf);
        }

        public void CatalogosTransaccionesFinancieras()
        {
            // Lista de Cuentas Contables
            var cuentasContables = catalogosM.ConsultarCuentasContables();

            List<SelectListItem> lstCuentasContables = new List<SelectListItem>();
            foreach (var item in cuentasContables)
            {
                lstCuentasContables.Add(new SelectListItem { Value = item.Codigo.ToString(), Text = item.Descripcion});
            }

            ViewBag.CuentasContables = lstCuentasContables;

            // Lista de Entidades Financieras
            var entidadesFinancieras = catalogosM.ConsultarEntidadesFinancieras();

            List<SelectListItem> lstEntidadesFinancieras = new List<SelectListItem>();
            foreach (var item in entidadesFinancieras)
            {
                lstEntidadesFinancieras.Add(new SelectListItem { Value = item.IdEntidad, Text = item.NombreEntidad });
            }

            ViewBag.EntidadesFinancieras = lstEntidadesFinancieras;
        }

        [HttpGet]
        public ActionResult DetallePedido(int idPedido)
        {
            var detalle = productosM.ConsultarDetallePedido(idPedido);
            return View(detalle);
        }

        [HttpPost]
        public ActionResult FacturarPedido(PedidoFactura ent)
        {
            var resultado = productosM.FacturarPedido(ent);
            if (!resultado)
                ViewBag.msj = "No fue posible facturar el pedido";
            return RedirectToAction("ListaPedidos", "Contabilidad");
        }

        [HttpPost]
        public ActionResult ContaPagoFactura(PedidoFactura ent)
        {
            var resultado = productosM.ContaPagoFactura(ent);
            if (!resultado)
            {
                return Json(new { success = false, message = "No fue posible facturar el pedido" });
            }

            var reporte = productosM.DatosRecibo(ent.idPagar);
            if (reporte != null)
            {
                // Devuelve la URL del archivo PDF en lugar de redirigir o descargar directamente
                var pdfUrl = Url.Action("GenReciboPdf", new { idPagar = ent.idPagar });
                return Json(new { success = true, pdfUrl = pdfUrl });
            }

            return Json(new { success = false, message = "Error al generar el reporte" });
        }

        public ActionResult GenReciboPdf(int idPagar)
        {
            var reporte = productosM.DatosRecibo(idPagar);
            if (reporte == null)
            {
                ViewBag.msj = "No fue posible generar el recibo";
                return RedirectToAction("ListaPedidos", "Contabilidad");
            }

            return new ViewAsPdf("ReciboPago", reporte)
            {
                FileName = "Recibo_" + idPagar + "_" + DateTime.Now.ToString("yyyy_MM_dd HH_mm_ss") + ".pdf",
                PageSize = Rotativa.Options.Size.A4,
                PageOrientation = Rotativa.Options.Orientation.Portrait,
            };
        }
        public ActionResult GenFacturaPdf(int idFactura)
        {
            FacturaPDF pdf = new FacturaPDF();
            pdf.Encabezado = productosM.DatosFacturaEncabezado(idFactura);
            pdf.Detalle = productosM.DatosFacturaDetalle(idFactura);
            if (pdf.Encabezado == null || pdf.Detalle == null)
            {
                ViewBag.msj = "No fue posible generar la factura";
                return RedirectToAction("ListaPedidos", "Contabilidad");
            }

            return new ViewAsPdf("FacturaPDF", pdf)
            {
                FileName = "Factura_" + idFactura + "_" + DateTime.Now.ToString("yyyy_MM_dd HH_mm_ss") + ".pdf",
                PageSize = Rotativa.Options.Size.A4,
                PageOrientation = Rotativa.Options.Orientation.Portrait,
            };
        }


        [HttpGet]
        public ActionResult CxC()
        {
            // Método pago
            var metodoPago = catalogosM.ConsultarMetodoPago();

            List<SelectListItem> lstMetodoPago = new List<SelectListItem>();
            foreach (var item in metodoPago)
            {
                lstMetodoPago.Add(new SelectListItem { Value = item.id.ToString(), Text = item.metodo.ToString() });
            }

            ViewBag.MetodoPago = lstMetodoPago;

            CatalogosTransaccionesFinancieras();

            CuentasCredito cc = new CuentasCredito();
            cc.ListaCxC = contabilidadM.ListaCxC();
            if (cc.ListaCxC != null)
                return View(cc);
            else
            {
                ViewBag.msj = "No fue posible obtener la lista";
                return View();
            }
        }

        [HttpPost]
        public ActionResult ContaCobroCxC(CuentasCredito ent)
        {
            var resultado = contabilidadM.ContaPagoCxC(ent);
            if (!resultado)
            {
                return Json(new { success = false, message = "No fue posible pagar la cuenta" });
            }

            var reporte = productosM.DatosRecibo(ent.Id_Cuenta);
            if (reporte != null)
            {
                // Devuelve la URL del archivo PDF en lugar de redirigir o descargar directamente
                var pdfUrl = Url.Action("GenReciboPdf", new { idPagar = ent.Id_Cuenta });
                return Json(new { success = true, pdfUrl = pdfUrl });
            }

            return Json(new { success = false, message = "Error al generar el reporte" });
        }

        [HttpGet]
        public ActionResult CxP()
        {
            // Método pago
            var metodoPago = catalogosM.ConsultarMetodoPago();

            List<SelectListItem> lstMetodoPago = new List<SelectListItem>();
            foreach (var item in metodoPago)
            {
                lstMetodoPago.Add(new SelectListItem { Value = item.id.ToString(), Text = item.metodo.ToString() });
            }

            ViewBag.MetodoPago = lstMetodoPago;

            CatalogosTransaccionesFinancieras();

            CuentasCredito cp = new CuentasCredito();
            cp.ListaCxP = contabilidadM.ListaCxP();
            if (cp.ListaCxP != null)
                return View(cp);
            else
            {
                ViewBag.msj = "No fue posible obtener la lista";
                return View();
            }
        }

        [HttpPost]
        public ActionResult ContaPagoCxP(CuentasCredito ent)
        {
            var resultado = contabilidadM.ContaPagoCxP(ent);
            if (!resultado)
                TempData["mensaje"] = "No fue posible pagar la cuenta";
            else
                TempData["mensaje"] = "Cuenta actualizada exitosamente";
            return RedirectToAction("CxP", "Contabilidad");
            
        }

        [HttpGet]
        public ActionResult ContaCierreCorte()
        {
            return View();

        }
        [HttpPost]
        public ActionResult ContaCierreCorte(RangoFecha fechas)
        {
            var cierre = contabilidadM.CierreContable(fechas);
            if (cierre != null)
            {
                fechas.cierreContable = cierre;
                return View(fechas);
            }
            //return View("ReporteITManagerResult", respuesta);

            else
            {
                ViewBag.msj = "No hay datos disponibles para el rango indicado";
                return View();
            }

        }

        [HttpGet]
        public ActionResult ReporteCierreContable(RangoFecha fechas)
        {
            RangoFecha reporte = new RangoFecha();
            reporte.cierreContable = contabilidadM.CierreContable(fechas);
            reporte.inicioCorte = fechas.inicioCorte;
            reporte.finCorte = fechas.finCorte;

            if (reporte.cierreContable != null)
                return new ViewAsPdf("CierreContableReporte", reporte)
                {
                    FileName = "ReporteCierreContable_" + DateTime.Now + ".pdf",
                    PageSize = Rotativa.Options.Size.A4,  // Tamaño de página A4
                    PageOrientation = Rotativa.Options.Orientation.Portrait,  // Orientación vertical
                };
            return RedirectToAction("ContaCierreCorte", "Contabilidad");
        }

        [HttpGet]
        public ActionResult RegistroTransacciones()
        {
            CatalogosTransaccionesFinancieras();
            return View();

        }

        //[HttpPost]
        //public ActionResult RegistroTransacciones(TransaccionFinanciera tf)
        //{
        //    for (int i = 0; i < tf.IdCuenta.Count; i++)
        //    {
        //        var registro = new TransaccionFinanciera
        //        {
        //            IdCuenta = tf.IdCuenta[i],
        //            Monto = tf.Monto[i]
        //        };
        //        //registro.GuardarEnBaseDatos();
        //    }

        //CatalogosTransaccionesFinancieras();

        //    return View();

        //}

        [HttpPost]
        public ActionResult RegistroTransacciones(List<TransaccionFinanciera> ListaTransacciones)
        {
            if (ListaTransacciones != null && ListaTransacciones.Count > 0)
            {
                int contador = 0;
                foreach (var transaccion in ListaTransacciones)
                {
                    var respuesta = contabilidadM.RegistroTransaccionFinanciera(transaccion);

                    if (!respuesta)
                    {
                        ViewBag.msj = "Error en el contador a partir de la línea" + contador;
                        CatalogosTransaccionesFinancieras();
                        return View();
                    }
                    contador++;
                }
            }

            return RedirectToAction("ContaCierreCorte", "Contabilidad");
        }

        [HttpGet]
        public ActionResult InformeVenta()
        {
            return View();
        }

        [HttpPost]
        public ActionResult InformeVenta(InformeVentas fechas)
        {
            var datos = contabilidadM.Informe_Ventas(fechas);
            if (datos != null)
            {
                return View(datos);
            }
            //return View("ReporteITManagerResult", respuesta);

            else
            {
                ViewBag.msj = "No hay datos disponibles para el rango indicado";
                return View();
            }
        }

        [HttpGet]
        public ActionResult InformeVentaPDF(InformeVentas fechas)
        {
            InformeVentas reporte = new InformeVentas();
            reporte = contabilidadM.Informe_Ventas(fechas);

            if (reporte != null)
                return new ViewAsPdf("InformeVentasPDF", reporte)
                {
                    FileName = "InformeVentas_" + DateTime.Now + ".pdf",
                    PageSize = Rotativa.Options.Size.A4,  // Tamaño de página A4
                    PageOrientation = Rotativa.Options.Orientation.Portrait,  // Orientación vertical
                };
            return RedirectToAction("InformeVenta", "Contabilidad");
        }

        [HttpGet]
        public ActionResult Inventario()
        {
            InventarioActualizacion ent = new InventarioActualizacion();
            ent.Productos = productosM.ConsultarProductos();
            return View(ent);
        }

        [HttpPost]
        public ActionResult ActualizarInventario(InventarioActualizacion ent)
        {
            var resultado = productosM.ActualizarInventario(ent);
            var IdUsuario = int.Parse(Session["IdUsuario"].ToString());
            notificacionesM.NuevaNotificacion(7, 6, 1, ent.id, IdUsuario); //Módulo 7, Notif básica 2, prioridad 1, usuario que lo generó
            if (!resultado)
            {
                return Json(new { success = false, message = "No fue posible facturar el pedido" });
            }
            
            return Json(new { success = true});
        }

        [HttpGet]
        public ActionResult HistorialModificacionesInventario()
        {
            List<InventarioActualizacion> historial = productosM.DatosHistoricos();
            return View(historial);
        }

        [HttpGet]
        public ActionResult AjusteInventarioPDF()
        {
            List<InventarioActualizacion> reporte = productosM.DatosHistoricos();

            if (reporte != null)
                return new ViewAsPdf("HistorialAjusteInventarioPDF", reporte)
                {
                    FileName = "HistorialAjusteInventario_" + DateTime.Now + ".pdf",
                    PageSize = Rotativa.Options.Size.A4,  // Tamaño de página A4
                    PageOrientation = Rotativa.Options.Orientation.Portrait,  // Orientación vertical
                };
            return RedirectToAction("HistorialModificacionesInventario", "Contabilidad");
        }

        [HttpGet]
        public ActionResult IncluirProductoInventario()
        {
            return View();
        }

        [HttpPost]
        public ActionResult IncluirProductoInventario(Producto producto)
        {
            var respuesta = contabilidadM.Registro_Producto(producto);

            // modificar todo esto de aquí para abajo y probar el método del modelo
            if (respuesta)
            {
                TempData["mensaje"] = "Producto registrado exitosamente";
                return RedirectToAction("Inventario", "Contabilidad");
            }

            else
            {
                ViewBag.msj = "No se ha podido registrar el producto";
                return View();
            }
        }

        [HttpGet]
        public ActionResult ListaPagosCxC(int id)
        {
            AjusteManualCuentasCredito ajuste = new AjusteManualCuentasCredito();
            ajuste.cuentasCreditos = contabilidadM.ListaPagosCxC(id);
            return View(ajuste);
        }

        [HttpPost]
        public ActionResult AjusteManualCxC(CuentasCredito ent)
        {
            ent.descripcion = "Ajuste manual: " + ent.descripcion;
            var resultado = contabilidadM.ContaPagoCxC(ent);
            var IdUsuario = int.Parse(Session["IdUsuario"].ToString());
            notificacionesM.NuevaNotificacion(7, 8, 1, ent.Id_Cuenta, IdUsuario); //Módulo 7, Notif básica 7, prioridad 1, usuario que lo generó
            //if (!resultado)
            //    TempData["mensaje"] = "No fue posible ajustar la cuenta por cobrar";
            //else
            //    TempData["mensaje"] = "Ajuste manual exitoso";
            return RedirectToAction("CxC", "Contabilidad");
        }

        [HttpGet]
        public ActionResult ListaPagosCxP(int id)
        {
            AjusteManualCuentasCredito ajuste = new AjusteManualCuentasCredito();
            ajuste.cuentasCreditos = contabilidadM.ListaPagosCxP(id);
            return View(ajuste);
        }

        [HttpPost]
        public ActionResult AjusteManualCxP(CuentasCredito ent)
        {
            ent.descripcion = "Ajuste manual: " + ent.descripcion;
            var resultado = contabilidadM.ContaPagoCxP(ent);
            var IdUsuario = int.Parse(Session["IdUsuario"].ToString());
            notificacionesM.NuevaNotificacion(7, 7, 1, ent.Id_Cuenta, IdUsuario); //Módulo 7, Notif básica 7, prioridad 1, usuario que lo generó
            //if (!resultado)
            //    TempData["mensaje"] = "No fue posible ajustar la cuenta por cobrar";
            //else
            //    TempData["mensaje"] = "Ajuste manual exitoso";
            return RedirectToAction("CxP", "Contabilidad");
        }


        [HttpGet]
        public ActionResult HistorialAjustesCxP()
        {
            var historial = contabilidadM.HistorialAjustesCxP();
            return View(historial);
        }

        [HttpGet]
        public ActionResult AjusteCxPPDF()
        {
            var reporte = contabilidadM.HistorialAjustesCxP();

            if (reporte != null)
                return new ViewAsPdf("HistorialAjustesCxPPDF", reporte)
                {
                    FileName = "HistorialAjustesCxP_" + DateTime.Now + ".pdf",
                    PageSize = Rotativa.Options.Size.A4,  // Tamaño de página A4
                    PageOrientation = Rotativa.Options.Orientation.Portrait,  // Orientación vertical
                };
            return RedirectToAction("HistorialAjustesCxP", "Contabilidad");
        }

        [HttpGet]
        public ActionResult HistorialAjustesCxC()
        {
            var historial = contabilidadM.HistorialAjustesCxC();
            return View(historial);
        }


        [HttpGet]
        public ActionResult AjusteCxCPDF()
        {
            var reporte = contabilidadM.HistorialAjustesCxC();

            if (reporte != null)
                return new ViewAsPdf("HistorialAjustesCxCPDF", reporte)
                {
                    FileName = "HistorialAjustesCxC_" + DateTime.Now + ".pdf",
                    PageSize = Rotativa.Options.Size.A4,  // Tamaño de página A4
                    PageOrientation = Rotativa.Options.Orientation.Portrait,  // Orientación vertical
                };
            return RedirectToAction("HistorialAjustesCxC", "Contabilidad");
        }


        [HttpGet]
        public ActionResult ContaEstadosFinancieros()
        {
            return View();

        }
        [HttpPost]
        public ActionResult ContaEstadosFinancieros(Entidades.EstadosFinancieros ef)
        {
            ef.BalanceGeneral = contabilidadM.BalanceGeneral(ef.mesCorte);
            ef.EstadoResultados = contabilidadM.EstadoResultados(ef.mesCorte);
            if (ef.BalanceGeneral != null && ef.EstadoResultados != null)
            {
                return View(ef);
            }
            //return View("ReporteITManagerResult", respuesta);

            else
            {
                ViewBag.msj = "No hay datos disponibles para el rango indicado";
                return View();
            }

        }

        [HttpGet]
        public ActionResult ContaEstadosFinancierosPDF(string fecha)
        {
            DateTime corte;
            DateTime.TryParseExact(fecha + "-01", "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out corte);

            Entidades.EstadosFinancieros ef = new Entidades.EstadosFinancieros();
            ef.BalanceGeneral = contabilidadM.BalanceGeneral(corte);
            ef.EstadoResultados = contabilidadM.EstadoResultados(corte);
            ef.mesCorte = corte;

            if (ef.BalanceGeneral != null && ef.EstadoResultados != null)
                return new ViewAsPdf("ContaEstadosFinancierosPDF", ef)
                {
                    FileName = "ReporteEstadosFinancieros_" + DateTime.Now + ".pdf",
                    PageSize = Rotativa.Options.Size.A4,  // Tamaño de página A4
                    PageOrientation = Rotativa.Options.Orientation.Portrait,  // Orientación vertical
                };
            return RedirectToAction("ContaEstadosFinancieros", "Contabilidad");
        }

        [HttpGet]
        public ActionResult VencimientoCuentas()
        {
            var notificaciones = contabilidadM.ListaVencimientos();
            return View(notificaciones);
        }

        [HttpGet]
        public ActionResult VencimientoCuentasPDF()
        {
            var reporte = contabilidadM.ListaVencimientos();

            if (reporte != null)
                return new ViewAsPdf("VencimientoCuentasPDF", reporte)
                {
                    FileName = "VencimientoCuentasPDF_" + DateTime.Now + ".pdf",
                    PageSize = Rotativa.Options.Size.A4,  // Tamaño de página A4
                    PageOrientation = Rotativa.Options.Orientation.Portrait,  // Orientación vertical
                };
            return RedirectToAction("VencimientoCuentas", "Contabilidad");
        }
    }
}