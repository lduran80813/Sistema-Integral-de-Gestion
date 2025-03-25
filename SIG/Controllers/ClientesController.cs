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
    public class ClientesController : Controller
    {
        ClientesModel clientesM = new ClientesModel();

        [HttpGet]
        public ActionResult Registro_Clientes()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Registro_Clientes(Entidades.Cliente cliente)
        {
            var respuesta = clientesM.Registro_Clientes(cliente);

            // modificar todo esto de aquí para abajo y probar el método del modelo
            if (respuesta)
            {
                TempData["mensaje"] = "Cliente registrado exitosamente";
                return RedirectToAction("ListaClientes", "Clientes");
            }

            else
            {
                ViewBag.msj = "No se ha podido registrar el cliente";
                return View();
            }
        }

        [HttpGet]
        public ActionResult ListaClientes()
        {
            var respuesta = clientesM.ListaClientes();
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
        public ActionResult ExportarClientes(string filtro)
        {

            var clientes = clientesM.ListaClientes();

            // filtro
            if (!string.IsNullOrEmpty(filtro))
            {
                clientes = clientes.Where(p => p.nombre.Contains(filtro)).ToList();
            }

            // verificación
            if (clientes.Count == 0)
            {
                TempData["msj"] = "No se encontraron clientes para exportar.";
                return RedirectToAction("ListaClientes");
            }

            // archivo Excel
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Clientes");
                worksheet.Cell(1, 1).Value = "Nombre";
                worksheet.Cell(1, 2).Value = "Teléfono";
                worksheet.Cell(1, 3).Value = "Correo Electrónico";
                worksheet.Cell(1, 4).Value = "Contacto";


                for (int i = 0; i < clientes.Count; i++)
                {
                    worksheet.Cell(i + 2, 1).Value = clientes[i].nombre;
                    worksheet.Cell(i + 2, 2).Value = clientes[i].telefono;
                    worksheet.Cell(i + 2, 3).Value = clientes[i].correo_electronico;
                }


                worksheet.Columns().AdjustToContents();


                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var fileName = "Clientes.xlsx";
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
        }

        [HttpGet]
        public ActionResult ActualizarCliente(int id)
        {
            // Voy a usar entidad, con un SP se podría evitar estar pasando los datos de un lado a otro
            var respuesta = clientesM.ObtenerCliente(id);
            if (respuesta == null)
            {
                ViewBag.msj = "No se ha podido conectar con la base de datos";
            }
            return View(respuesta);
        }

        [HttpPost]
        public ActionResult ActualizarCliente(Entidades.Cliente cliente)
        {
            var respuesta = clientesM.ActualizarCliente(cliente);
            if (respuesta)
            {
                TempData["mensaje"] = "Cliente actualizado exitosamente";
                return RedirectToAction("ListaClientes", "Clientes");
            }
            else
            {
                ViewBag.msj = "No se ha podido actualizar el cliente";
                return View();
            }
        }

        [HttpPost]
        public ActionResult EliminarCliente(Entidades.Cliente cliente)
        {
            var respuesta = clientesM.EliminarCliente(cliente);
            if (respuesta)
            {
                TempData["mensaje"] = "Cliente eliminado correctamente";
            }
            else
            {
                TempData["mensaje"] = "No se ha podido eliminar el Cliente";
            }
            return RedirectToAction("ListaClientes", "Clientes");
        }
    }
}