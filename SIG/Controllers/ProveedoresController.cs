using ClosedXML.Excel;
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

        // Pendiente el POST para actualizar datos
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

        [HttpGet]
        public ActionResult ConsultaEstadoFinanciero()
        {

            return View();


        }


        [HttpPost]
        public ActionResult ConsultaEstadoFinanciero(Entidades.Proveedor proveedor)
        {
            var estadoFinanciero = proveedoresM.ObtenerEstadoFinanciero(proveedor.id);

            if (!string.IsNullOrEmpty(estadoFinanciero.MensajeError))
            {
                ViewBag.Error = estadoFinanciero.MensajeError;
                return View();
            }

            return View(estadoFinanciero);
        }

      

        [HttpGet]
        public ActionResult ConsultaEstadoFinancieroConjunto(List<int> ids)
        {
            var estadosFinancieros = proveedoresM.ObtenerEstadoFinancieroConjunto(ids);

            if (estadosFinancieros.Count == 0 || estadosFinancieros.Any(e => !string.IsNullOrEmpty(e.MensajeError)))
            {
                ViewBag.Error = "No hay datos disponibles o ocurrió un error al consultar.";
                return View();
            }

            return View(estadosFinancieros);
        }
    }
}