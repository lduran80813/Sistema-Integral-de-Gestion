using ClosedXML.Excel;
using SIG.Entidades;
using SIG.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SIG.Controllers
{
    public class ProveedoresController : Controller
    {
        ProveedoresModel proveedoresM = new ProveedoresModel();

        [HttpGet]
        public ActionResult Registro_Proveedores()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Registro_Proveedores(Entidades.Proveedor proveedor)
        {
            var respuesta = proveedoresM.Registro_Proveedores(proveedor);

            // modificar todo esto de aquí para abajo y probar el método del modelo
            if (respuesta)
            {
                TempData["mensaje"] = "Proveedor registrado exitosamente";
                return RedirectToAction("ListaProveedores", "Proveedores");
            }

            else
            {
                ViewBag.msj = "No se ha podido registrar el proveedor";
                return View();
            }
        }

        [HttpGet]
        public ActionResult ListaProveedores()
        {
            var respuesta = proveedoresM.ListaProveedores();
            if (respuesta == null)
            {
                ViewBag.msj = "No se ha podido conectar con la base de datos";
                return View();
            }

            if (TempData["mensaje"] != null)
            {
                var mensaje = TempData["mensaje"].ToString();
                ViewBag.msj = mensaje;
            }

            return View(respuesta);
        }

        
        [HttpGet]
        public ActionResult ExportarProveedores(string filtro)
        {
           
            var proveedores = proveedoresM.ListaProveedores();

            // filtro
            if (!string.IsNullOrEmpty(filtro))
            {
                proveedores = proveedores.Where(p => p.nombre.Contains(filtro)).ToList();
            }

            // verificación
            if (proveedores.Count == 0)
            {
                TempData["msj"] = "No se encontraron proveedores para exportar.";
                return RedirectToAction("ListaProveedores");
            }

            // archivo Excel
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Proveedores");
                worksheet.Cell(1, 1).Value = "Nombre";
                worksheet.Cell(1, 2).Value = "Teléfono";
                worksheet.Cell(1, 3).Value = "Correo Electrónico";
                worksheet.Cell(1, 4).Value = "Contacto";

                
                for (int i = 0; i < proveedores.Count; i++)
                {
                    worksheet.Cell(i + 2, 1).Value = proveedores[i].nombre;
                    worksheet.Cell(i + 2, 2).Value = proveedores[i].telefono;
                    worksheet.Cell(i + 2, 3).Value = proveedores[i].correo_electronico;
                    worksheet.Cell(i + 2, 4).Value = proveedores[i].nombre_contacto;
                }

                
                worksheet.Columns().AdjustToContents();

                
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var fileName = "Proveedores.xlsx";
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
        }



        [HttpGet]
        public ActionResult ActualizarProveedor(int id)
        {
            // Voy a usar entidad, con un SP se podría evitar estar pasando los datos de un lado a otro
            var respuesta = proveedoresM.ObtenerProveedor(id);
            if (respuesta == null)
            {
                ViewBag.msj = "No se ha podido conectar con la base de datos";
            }
            return View(respuesta);
        }

        [HttpPost]
        public ActionResult ActualizarProveedor(Entidades.Proveedor proveedor)
        {
            var respuesta = proveedoresM.ActualizarProveedor(proveedor);
            if (respuesta)
            {
                TempData["mensaje"] = "Proveedor actualizado exitosamente";
                return RedirectToAction("ListaProveedores", "Proveedores");
            }
            else
            {
                ViewBag.msj = "No se ha podido actualizar el proveedor";
                return View();
            }
        }

        [HttpPost]
        public ActionResult EliminarProveedor(Entidades.Proveedor proveedor)
        {
            var respuesta = proveedoresM.EliminarProveedor(proveedor);
            if (respuesta)
            {
                TempData["mensaje"] = "Proveedor eliminado correctamente";
            }
            else
            {
                TempData["mensaje"] = "No se ha podido eliminar el Proveedor";
            }
            return RedirectToAction("ListaProveedores", "Proveedores");
        }

        //Específico

        [HttpGet]
        public ActionResult ConsultarEstadoFinanciero()
        {
            var proveedores = proveedoresM.ListaProveedores();
            ViewBag.Proveedores = proveedores;

            return View();
        }

        [HttpPost]
        public ActionResult ConsultarEstadoFinanciero(Entidades.ConsultaEstadoFinanciero consulta, int[] ProveedorIds)
        {
            var proveedores = proveedoresM.ListaProveedores();
            ViewBag.Proveedores = proveedores;
        
            if (ProveedorIds != null && ProveedorIds.Length > 0)
            {              
                consulta.ProveedorIds = ProveedorIds.ToList(); 

                var estadoFinanciero = proveedoresM.ObtenerEstadoFinanciero(consulta);

                if (estadoFinanciero.Any())
                {
                    return View(estadoFinanciero); 
                }

                ViewBag.msj = "No se encontraron datos financieros para los proveedores seleccionados en el rango de fechas.";
            }
            else
            {
                ViewBag.msj = "Por favor, seleccione al menos un proveedor.";
            }

            return View();
        }

        //En conjunto
        [HttpGet]
        public ActionResult ConsultarEstadoFinancieroConjunto()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ConsultarEstadoFinancieroConjunto(DateTime fechaInicio, DateTime fechaFin)
        {
            
            var consulta = new ConsultaEstadoFinanciero
            {
                FechaInicio = fechaInicio,
                FechaFin = fechaFin,
                ProveedorIds = proveedoresM.ListaProveedores().Select(p => p.id).ToList()
            };

            var estadoFinanciero = proveedoresM.ObtenerEstadoFinancieroConjunto(consulta);

            if (estadoFinanciero.Any())
            {
                var totalComprasContado = estadoFinanciero.Sum(x => x.TotalComprasContado);
                var totalComprasCredito = estadoFinanciero.Sum(x => x.TotalComprasCredito);
                var totalCompras = estadoFinanciero.Sum(x => x.TotalCompras);
                var resumenFinanciero = new List<ProveedorFinanciero>
        {
            new ProveedorFinanciero
            {
                TotalComprasContado = totalComprasContado,
                TotalComprasCredito = totalComprasCredito,
                TotalCompras = totalCompras,
                FechaCorte = fechaFin 
            }
        };

                return View("ConsultarEstadoFinancieroConjunto", resumenFinanciero);
            }

            ViewBag.msj = "No se encontraron datos financieros para los proveedores seleccionados en el rango de fechas.";
            return View();
        }

        [HttpGet]
        public ActionResult ExportarEstadoFinancieroConjunto(DateTime fechaInicio, DateTime fechaFin)
        {
            try
            {
                
                var consulta = new ConsultaEstadoFinanciero
                {
                    FechaInicio = fechaInicio,
                    FechaFin = fechaFin,
                    ProveedorIds = proveedoresM.ListaProveedores().Select(p => p.id).ToList()
                };

                var estadoFinanciero = proveedoresM.ObtenerEstadoFinancieroConjunto(consulta);

                if (!estadoFinanciero.Any())
                {
                    TempData["msj"] = "No se encontraron datos para exportar.";
                    return RedirectToAction("ConsultarEstadoFinancieroConjunto");
                }

                var totalComprasContado = estadoFinanciero.Sum(x => x.TotalComprasContado);
                var totalComprasCredito = estadoFinanciero.Sum(x => x.TotalComprasCredito);
                var totalCompras = estadoFinanciero.Sum(x => x.TotalCompras);

                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Reporte Financiero Conjunto");

                    worksheet.Cell(1, 1).Value = "Total Compras Contado";
                    worksheet.Cell(1, 2).Value = "Total Compras Crédito";
                    worksheet.Cell(1, 3).Value = "Total Compras";
                    worksheet.Cell(1, 4).Value = "Fecha Corte";

                    worksheet.Cell(2, 1).Value = totalComprasContado;
                    worksheet.Cell(2, 2).Value = totalComprasCredito;
                    worksheet.Cell(2, 3).Value = totalCompras;
                    worksheet.Cell(2, 4).Value = fechaFin.ToShortDateString();

                    worksheet.Columns().AdjustToContents();

                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        var fileName = "ReporteFinancieroConjunto.xlsx";
                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["msj"] = $"Ocurrió un error al generar el reporte: {ex.Message}";
                return RedirectToAction("ConsultarEstadoFinancieroConjunto");
            }
        }
    }
}