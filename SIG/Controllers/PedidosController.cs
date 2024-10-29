using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using ClosedXML.Excel;
using SIG.Entidades;
using SIG.Models;

namespace SIG.Controllers
{
    public class PedidosController : Controller
    {
        private readonly PedidosModel pedidosModel = new PedidosModel();
        private readonly ProveedoresModel proveedoresModel = new ProveedoresModel();

        [HttpGet]
        public ActionResult RegistrarPedido()
        {
            CargarProveedoresYEstados(excluirEstadoCancelado: true);
            return View(new Entidades.Pedido());
        }

        [HttpPost]
        public ActionResult RegistrarPedido(Entidades.Pedido pedido)
        {
            if (pedido == null || pedido.total_pedido < 0)
            {
                ViewBag.msj = pedido == null ? "No se pudo procesar el pedido." : "El total de la compra no puede ser negativo.";
                CargarProveedoresYEstados(excluirEstadoCancelado: true);
                return View(pedido ?? new Entidades.Pedido());
            }

            if (!pedidosModel.EsProveedorActivo(pedido.proveedor_id))
            {
                ViewBag.msj = "El proveedor seleccionado está inactivo.";
                CargarProveedoresYEstados(excluirEstadoCancelado: true);
                return View(pedido);
            }

            if (ModelState.IsValid && pedidosModel.RegistrarPedido(pedido))
            {
                TempData["mensaje"] = "Pedido registrado exitosamente.";
                return RedirectToAction("ListaPedidos");
            }

            ViewBag.msj = "Formulario incompleto o datos inválidos.";
            CargarProveedoresYEstados(excluirEstadoCancelado: true);
            return View(pedido);
        }

        [HttpGet]
        public ActionResult ListaPedidos()
        {
            try
            {
                var pedidos = pedidosModel.ListaPedidos();
                ViewBag.EstadoDescripciones = ObtenerDiccionarioEstados();
                return View(pedidos);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Se produjo un error al cargar la lista de pedidos: " + ex.Message;
                return View("Error");
            }
        }

        [HttpGet]
        public ActionResult EliminarPedido(int id)
        {
            try
            {
                TempData["mensaje"] = pedidosModel.CancelarPedido(id) ? "Pedido cancelado exitosamente." : "Error al cancelar el pedido.";
            }
            catch (Exception ex)
            {
                TempData["mensaje"] = "Error: " + ex.Message;
            }
            return RedirectToAction("ListaPedidos");
        }

        private void CargarProveedoresYEstados(bool excluirEstadoCancelado = false)
        {
            ViewBag.Proveedores = proveedoresModel.ListaProveedores();
            var estadosCompra = pedidosModel.ListaEstadosCompra();
            if (excluirEstadoCancelado) estadosCompra = estadosCompra.Where(e => e.Id != 4).ToList();
            ViewBag.EstadosCompra = estadosCompra;
        }

        [HttpGet]
        public ActionResult ModificarPedido(int id)
        {
            var pedido = pedidosModel.ObtenerPedidoPorId(id);
            if (pedido == null)
            {
                return HttpNotFound();
            }

            CargarProveedoresYEstados();
            return View(pedido);
        }

        [HttpPost]
        public ActionResult ModificarPedido(Entidades.Pedido pedido)
        {
            if (pedido.EstadoCompraId <= 0 || !pedidosModel.EsEstadoCompraValido(pedido.EstadoCompraId))
            {
                ViewBag.msj = "El estado de compra seleccionado no es válido.";
                CargarProveedoresYEstados();
                return View(pedido);
            }

            if (ModelState.IsValid && pedidosModel.ModificarPedido(pedido))
            {
                TempData["mensaje"] = "Pedido modificado exitosamente.";
                return RedirectToAction("ListaPedidos");
            }

            ViewBag.msj = "Formulario incompleto o datos inválidos.";
            CargarProveedoresYEstados();
            return View(pedido);
        }

        private Dictionary<int, string> ObtenerDiccionarioEstados()
        {
            return pedidosModel.ListaEstadosCompra().ToDictionary(e => e.Id, e => e.Nombre);
        }

        public ActionResult DetallePedido(int id)
        {
            var pedido = pedidosModel.ObtenerPedidoPorId(id);
            if (pedido == null)
            {
                return HttpNotFound();
            }

            var historial = pedidosModel.ObtenerHistorialPedido(id);
            ViewBag.Historial = historial;
            ViewBag.EstadoDescripciones = ObtenerDiccionarioEstados();
            return View(pedido);
        }

        [HttpGet]
        public ActionResult VerHistorial(int id)
        {
            try
            {
                var historial = pedidosModel.ObtenerHistorialPedido(id);
                if (historial == null || !historial.Any())
                {
                    ViewBag.ErrorMessage = "No se encontró historial para el pedido seleccionado.";
                    return View("Error");
                }

                ViewBag.EstadoDescripciones = ObtenerDiccionarioEstados();
                return View(historial);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Se produjo un error al cargar el historial de pedidos: " + ex.Message;
                return View("Error");
            }
        }

        [HttpGet]
        public ActionResult GenerarReporteForm()
        {
            ViewBag.Proveedores = proveedoresModel.ListaProveedores();
            return View();
        }

        [HttpPost]
        public ActionResult GenerarReporte(int proveedorId)
        {
            if (proveedorId <= 0)
            {
                ViewBag.ErrorMessage = "Por favor, seleccione un proveedor válido.";
                ViewBag.Proveedores = proveedoresModel.ListaProveedores();
                return View("GenerarReporteForm");
            }

            var reporte = pedidosModel.GenerarReporte(proveedorId);
            ViewBag.Proveedores = proveedoresModel.ListaProveedores();
            ViewBag.ProveedorId = proveedorId; 

            return View("ReporteGenerado", reporte);
        }

        [HttpGet]
        public ActionResult ExportarReporte(int? proveedorId)
        {
            if (!proveedorId.HasValue || proveedorId <= 0)
            {
                return Json(new { success = false, message = "Proveedor no válido." }, JsonRequestBehavior.AllowGet);
            }

            var reportes = pedidosModel.GenerarReporte(proveedorId.Value);

            if (reportes == null || !reportes.Any())
            {
                return Json(new { success = false, message = "No hay datos para exportar." }, JsonRequestBehavior.AllowGet);
            }

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Reporte de Pedidos");
                worksheet.Cell(1, 1).Value = "ID";
                worksheet.Cell(1, 2).Value = "Fecha de Pedido";
                worksheet.Cell(1, 3).Value = "Proveedor";
                worksheet.Cell(1, 4).Value = "Total";
                worksheet.Cell(1, 5).Value = "Evaluación";

                int row = 2;
                foreach (var pedido in reportes)
                {
                    worksheet.Cell(row, 1).Value = pedido.PedidoId;
                    worksheet.Cell(row, 2).Value = pedido.FechaPedido.ToString("dd/MM/yyyy");
                    worksheet.Cell(row, 3).Value = pedido.Proveedor;
                    worksheet.Cell(row, 4).Value = pedido.Total.ToString("C");
                    worksheet.Cell(row, 5).Value = pedido.Evaluacion;
                    row++;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Reporte_Pedidos.xlsx");
                }
            }
        }
    }
}
