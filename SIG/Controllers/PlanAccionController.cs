using SIG.Models;
using SIG.Entidades;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using iTextSharp.text.pdf.draw;

namespace SIG.Controllers
{
    public class PlanDeAccionController : Controller
    {
        private readonly PlanDeAccionModel _planDeAccionModel;

        public PlanDeAccionController()
        {
            _planDeAccionModel = new PlanDeAccionModel();
        }

        // Acción GET para listar los Planes de Acción del usuario
        [HttpGet]
        public ActionResult ListarPlanesUsuario()
        {
            var planes = _planDeAccionModel.ListarPlanesUsuario();
            return View(planes);
        }

        // Acción GET para registrar un nuevo Plan de Acción
        [HttpGet]
        public ActionResult Registro()
        {
            var empleados = _planDeAccionModel.ListarEmpleados();
            ViewBag.Empleados = empleados ?? new List<SelectListItem>();

            var estadosPlan = new List<SelectListItem>
            {
                new SelectListItem { Text = "En Curso", Value = "En Curso" },
                new SelectListItem { Text = "Pendiente", Value = "Pendiente" },
                new SelectListItem { Text = "Completado", Value = "Completado" }
            };
            ViewBag.Estados = estadosPlan;

            var estadosTarea = _planDeAccionModel.ListarEstadosTarea();
            ViewBag.EstadosTarea = estadosTarea ?? new List<SelectListItem>();

            var plan = new PlanDeAccion
            {
                Tareas = new List<Tarea> { new Tarea() }
            };

            return View(plan);
        }

        // Acción POST para registrar un nuevo Plan de Acción
        [HttpPost]
        public JsonResult RegistroAjax(PlanDeAccion plan)
        {
            if (ModelState.IsValid)
            {
                bool resultado = _planDeAccionModel.InsertarPlan(plan);

                if (resultado)
                {
                    return Json(new { success = true, message = "Plan de Acción registrado con éxito." });
                }
                else
                {
                    return Json(new { success = false, message = "Hubo un problema al registrar el Plan de Acción." });
                }
            }
            return Json(new { success = false, message = "Datos proporcionados no válidos." });
        }


        // Acción GET para ver las tareas asociadas a un Plan de Acción
        [HttpGet]
        public ActionResult VerTareas(int idPlan)
        {
            var plan = _planDeAccionModel.VerPlan(idPlan);
            if (plan == null)
            {
                return HttpNotFound();
            }

            ViewBag.Plan = plan;
            return View(plan.Tareas);
        }

        // Acción GET para editar un Plan de Acción
        [HttpGet]
        public ActionResult Editar(int idPlan)
        {
            var plan = _planDeAccionModel.VerPlan(idPlan);
            if (plan == null)
            {
                return HttpNotFound();
            }

            var empleados = _planDeAccionModel.ListarEmpleados();
            ViewBag.Empleados = empleados ?? new List<SelectListItem>();

            var estadosPlan = new List<SelectListItem>
            {
                new SelectListItem { Text = "En Curso", Value = "En Curso" },
                new SelectListItem { Text = "Pendiente", Value = "Pendiente" },
                new SelectListItem { Text = "Completado", Value = "Completado" }
            };
            ViewBag.Estados = estadosPlan;

            var estadosTarea = _planDeAccionModel.ListarEstadosTarea();
            ViewBag.EstadosTarea = estadosTarea ?? new List<SelectListItem>();

            return View(plan);
        }

        // Acción POST para editar un Plan de Acción
        [HttpPost]
        public ActionResult Editar(PlanDeAccion plan)
        {
            if (ModelState.IsValid)
            {
                bool resultado = _planDeAccionModel.ActualizarPlan(plan);
                if (resultado)
                {
                    TempData["Message"] = "Plan de Acción actualizado con éxito.";
                    return RedirectToAction("ListarPlanesUsuario");
                }
                else
                {
                    ModelState.AddModelError("", "Hubo un problema al actualizar el Plan de Acción.");
                }
            }

            var empleados = _planDeAccionModel.ListarEmpleados();
            ViewBag.Empleados = empleados ?? new List<SelectListItem>();

            var estadosTarea = _planDeAccionModel.ListarEstadosTarea();
            ViewBag.EstadosTarea = estadosTarea ?? new List<SelectListItem>();

            return View(plan);
        }

        [HttpPost]
        public JsonResult GuardarEvaluacion(int tareaId, int calificacion, string observacion)
        {
            try
            {
                var tarea = _planDeAccionModel.ObtenerTareaPorId(tareaId);
                if (tarea == null)
                {
                    return Json(new { success = false, message = "La tarea no existe." });
                }

                tarea.Calificacion = calificacion;
                tarea.Observacion = observacion;

                bool resultado = _planDeAccionModel.GuardarEvaluacionTarea(tareaId, calificacion, observacion);

                if (resultado)
                {
                    return Json(new { success = true, message = "Evaluación guardada correctamente." });
                }

                return Json(new { success = false, message = "No se pudo guardar la evaluación." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public ActionResult ExportarPDF()
        {
            var planes = _planDeAccionModel.ListarPlanesUsuario();

            foreach (var plan in planes)
            {
                plan.Tareas = _planDeAccionModel.VerPlan(plan.Id)?.Tareas ?? new List<Tarea>();
            }

            using (MemoryStream ms = new MemoryStream())
            {
                Document pdfDoc = new Document(PageSize.A4, 36, 36, 36, 36);
                PdfWriter writer = PdfWriter.GetInstance(pdfDoc, ms);

                pdfDoc.Open();

                Font titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 20);
                Font subTitleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14);
                Font tableHeaderFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
                Font textFont = FontFactory.GetFont(FontFactory.HELVETICA, 11);

                Paragraph title = new Paragraph("Rajasa de Coro", titleFont)
                {
                    Alignment = Element.ALIGN_CENTER,
                    SpacingAfter = 10
                };
                pdfDoc.Add(title);

                Paragraph subTitle = new Paragraph("Listado de Planes de Acción y Tareas Asociadas", subTitleFont)
                {
                    Alignment = Element.ALIGN_CENTER,
                    SpacingAfter = 20
                };
                pdfDoc.Add(subTitle);

                foreach (var plan in planes)
                {
                    Paragraph planTitle = new Paragraph($"Plan: {plan.NombrePlan}", subTitleFont)
                    {
                        SpacingBefore = 10,
                        SpacingAfter = 5
                    };
                    pdfDoc.Add(planTitle);

                    PdfPTable planTable = new PdfPTable(2) { WidthPercentage = 100 };
                    planTable.AddCell(new PdfPCell(new Phrase("Descripción:", tableHeaderFont)) { Border = 0 });
                    planTable.AddCell(new PdfPCell(new Phrase(plan.DescripcionPlan ?? "Sin descripción", textFont)) { Border = 0 });

                    planTable.AddCell(new PdfPCell(new Phrase("Fecha de Inicio:", tableHeaderFont)) { Border = 0 });
                    planTable.AddCell(new PdfPCell(new Phrase(plan.FechaInicio.ToString("dd/MM/yyyy"), textFont)) { Border = 0 });

                    planTable.AddCell(new PdfPCell(new Phrase("Fecha de Finalización:", tableHeaderFont)) { Border = 0 });
                    planTable.AddCell(new PdfPCell(new Phrase(plan.FechaFinalizacion?.ToString("dd/MM/yyyy") ?? "No disponible", textFont)) { Border = 0 });

                    planTable.AddCell(new PdfPCell(new Phrase("Estado:", tableHeaderFont)) { Border = 0 });
                    planTable.AddCell(new PdfPCell(new Phrase(plan.Estado, textFont)) { Border = 0 });

                    planTable.AddCell(new PdfPCell(new Phrase("Responsable ID:", tableHeaderFont)) { Border = 0 });
                    planTable.AddCell(new PdfPCell(new Phrase(plan.ResponsableId.ToString(), textFont)) { Border = 0 });

                    pdfDoc.Add(planTable);

                    if (plan.Tareas != null && plan.Tareas.Any())
                    {
                        Paragraph tasksTitle = new Paragraph("Tareas Asociadas", subTitleFont)
                        {
                            SpacingBefore = 10,
                            SpacingAfter = 5
                        };
                        pdfDoc.Add(tasksTitle);

                        PdfPTable tasksTable = new PdfPTable(5) { WidthPercentage = 100 };
                        tasksTable.AddCell(new PdfPCell(new Phrase("Descripción", tableHeaderFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                        tasksTable.AddCell(new PdfPCell(new Phrase("Responsable", tableHeaderFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                        tasksTable.AddCell(new PdfPCell(new Phrase("Estado", tableHeaderFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                        tasksTable.AddCell(new PdfPCell(new Phrase("Calificación", tableHeaderFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                        tasksTable.AddCell(new PdfPCell(new Phrase("Observación", tableHeaderFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });

                        foreach (var tarea in plan.Tareas)
                        {
                            tasksTable.AddCell(new PdfPCell(new Phrase(tarea.DescripcionTarea ?? "Sin descripción", textFont)));
                            tasksTable.AddCell(new PdfPCell(new Phrase(tarea.Responsable?.Nombre ?? "No asignado", textFont)));
                            tasksTable.AddCell(new PdfPCell(new Phrase(tarea.EstadoTarea?.Descripcion ?? "Sin estado", textFont)));
                            tasksTable.AddCell(new PdfPCell(new Phrase(tarea.Calificacion?.ToString() ?? "N/A", textFont)));
                            tasksTable.AddCell(new PdfPCell(new Phrase(tarea.Observacion ?? "Sin observación", textFont)));
                        }

                        pdfDoc.Add(tasksTable);
                    }
                    else
                    {
                        Paragraph noTasks = new Paragraph("No hay tareas asociadas a este plan.", textFont)
                        {
                            SpacingBefore = 5,
                            SpacingAfter = 10
                        };
                        pdfDoc.Add(noTasks);
                    }

                    pdfDoc.Add(new Chunk(new LineSeparator()));
                }

                Paragraph footer = new Paragraph("Documento generado el " + DateTime.Now.ToString("dd/MM/yyyy"), textFont)
                {
                    Alignment = Element.ALIGN_CENTER,
                    SpacingBefore = 20
                };
                pdfDoc.Add(footer);

                pdfDoc.Close();
                return File(ms.ToArray(), "application/pdf", "PlanesDeAccion_RajasaDeCoro.pdf");
            }
        }
    }
}
