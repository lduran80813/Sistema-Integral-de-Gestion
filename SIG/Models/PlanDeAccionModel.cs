using SIG.BaseDatos;
using SIG.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SIG.Models
{
    public class PlanDeAccionModel
    {
        // Método para obtener el nombre completo de un empleado a partir de su ID
        public string ObtenerEmpleadoNombre(int responsableId)
        {
            using (var context = new SistemaIntegralGestionEntities())
            {
                var empleado = context.Empleado.FirstOrDefault(e => e.id == responsableId);
                return empleado != null ? $"{empleado.nombre} {empleado.apellidos}" : "Desconocido";
            }
        }

        // Método para listar los empleados en formato SelectListItem (para dropdown)
        public IEnumerable<SelectListItem> ListarEmpleados()
        {
            using (var context = new SistemaIntegralGestionEntities())
            {
                var empleados = context.Empleado
                                       .Select(e => new
                                       {
                                           e.id,
                                           e.nombre,
                                           e.apellidos
                                       })
                                       .ToList();

                var empleadosList = empleados.Select(e => new SelectListItem
                {
                    Value = e.id.ToString(),
                    Text = $"{e.nombre} {e.apellidos}"
                }).ToList();

                return empleadosList;
            }
        }

        // Método para obtener los estados de las tareas
        public IEnumerable<SelectListItem> ListarEstadosTarea()
        {
            using (var context = new SistemaIntegralGestionEntities())
            {
                var estadosTarea = context.PDA_Estado
                                          .Select(e => new SelectListItem
                                          {
                                              Value = e.id.ToString(),
                                              Text = e.descripcion_estado
                                          })
                                          .ToList();

                return estadosTarea;
            }
        }

        // Insertar un nuevo Plan de Acción
        public bool InsertarPlan(Entidades.PlanDeAccion plan)
        {
            var rowsAffected = 0;

            using (var context = new SistemaIntegralGestionEntities())
            {
                int idSesion = int.Parse(HttpContext.Current.Session["IdUsuario"].ToString());

                var tPlan = new SIG.BaseDatos.PlanDeAccion
                {
                    id = idSesion,
                    nombre_plan = plan.NombrePlan,
                    descripcion_plan = plan.DescripcionPlan,
                    fecha_inicio = plan.FechaInicio,
                    fecha_finalizacion = plan.FechaFinalizacion ?? DateTime.Now.AddMonths(1),
                    estado = plan.Estado,
                    ResponsableId = plan.ResponsableId
                };

                context.PlanDeAccion.Add(tPlan);
                rowsAffected = context.SaveChanges();

                if (rowsAffected > 0 && plan.Tareas != null)
                {
                    foreach (var tarea in plan.Tareas)
                    {
                        var tTarea = new SIG.BaseDatos.PDA_Tarea
                        {
                            descripcion_tarea = tarea.DescripcionTarea,
                            responsable_id = tarea.ResponsableId,
                            estado_tarea = tarea.EstadoTareaId,
                            plan_id = tPlan.id
                        };

                        context.PDA_Tarea.Add(tTarea);
                    }

                    rowsAffected = context.SaveChanges();
                }
            }

            return rowsAffected > 0;
        }

        // Listar los Planes de Acción del usuario
        public List<Entidades.PlanDeAccion> ListarPlanesUsuario()
        {
            using (var context = new SistemaIntegralGestionEntities())
            {
                return (from x in context.PlanDeAccion
                        select new Entidades.PlanDeAccion
                        {
                            Id = x.id,
                            NombrePlan = x.nombre_plan,
                            DescripcionPlan = x.descripcion_plan,
                            FechaInicio = (DateTime)x.fecha_inicio,
                            FechaFinalizacion = x.fecha_finalizacion,
                            Estado = x.estado,
                            ResponsableId = x.ResponsableId ?? 0
                        }).ToList();
            }
        }

        // Método para obtener el nombre del estado de la tarea a partir de su ID
        public string ObtenerEstadoTareaNombre(int estadoTareaId)
        {
            using (var context = new SistemaIntegralGestionEntities())
            {
                var estadoTarea = context.PDA_Estado.FirstOrDefault(e => e.id == estadoTareaId);
                return estadoTarea != null ? estadoTarea.descripcion_estado : "Desconocido";
            }
        }

        // Obtener un Plan de Acción por su ID
        public Entidades.PlanDeAccion VerPlan(int idPlan)
        {
            using (var context = new SistemaIntegralGestionEntities())
            {

                var plan = (from x in context.PlanDeAccion
                            where x.id == idPlan
                            select new Entidades.PlanDeAccion
                            {
                                Id = x.id,
                                NombrePlan = x.nombre_plan,
                                DescripcionPlan = x.descripcion_plan,
                                FechaInicio = (DateTime)x.fecha_inicio,
                                FechaFinalizacion = x.fecha_finalizacion,
                                Estado = x.estado,
                                ResponsableId = x.ResponsableId ?? 0,
                            }).FirstOrDefault();

                if (plan == null)
                {
                    return null;
                }

                var tareas = (from t in context.PDA_Tarea
                              where t.plan_id == idPlan
                              select new
                              {
                                  t.id,
                                  t.descripcion_tarea,
                                  t.responsable_id,
                                  t.estado_tarea,
                                  t.calificacion,
                                  t.observacion
                              }).ToList();

                plan.Tareas = tareas.Select(t => new Tarea
                {
                    Id = t.id,
                    DescripcionTarea = t.descripcion_tarea,
                    ResponsableId = t.responsable_id ?? 0,
                    EstadoTareaId = t.estado_tarea ?? 0,
                    EstadoTarea = new PDAEstado
                    {
                        Id = t.estado_tarea ?? 0
                    },
                    Calificacion = t.calificacion,
                    Observacion = t.observacion

                }).ToList();

                var responsablesIds = plan.Tareas.Select(t => t.ResponsableId).Distinct().ToList();

                var empleados = context.Empleado
                    .Where(e => responsablesIds.Contains(e.id))
                    .ToList();

                foreach (var tarea in plan.Tareas)
                {
                    var empleado = empleados.FirstOrDefault(e => e.id == tarea.ResponsableId);
                    if (empleado != null)
                    {
                        tarea.Responsable = new PDAEmpleado
                        {
                            Id = empleado.id,
                            Nombre = $"{empleado.nombre} {empleado.apellidos}"
                        };
                    }

                    tarea.EstadoTarea.Descripcion = ObtenerEstadoTareaNombre(tarea.EstadoTareaId);
                }

                return plan;
            }
        }

        // Actualizar un Plan de Acción existente
        public bool ActualizarPlan(Entidades.PlanDeAccion plan)
        {
            var rowsAffected = 0;

            using (var context = new SistemaIntegralGestionEntities())
            {

                var planExistente = context.PlanDeAccion.FirstOrDefault(p => p.id == plan.Id);
                if (planExistente != null)
                {

                    planExistente.nombre_plan = plan.NombrePlan;
                    planExistente.descripcion_plan = plan.DescripcionPlan;
                    planExistente.fecha_inicio = plan.FechaInicio;
                    planExistente.fecha_finalizacion = plan.FechaFinalizacion;
                    planExistente.estado = plan.Estado;
                    planExistente.ResponsableId = plan.ResponsableId;

                    rowsAffected = context.SaveChanges();
                }
            }

            return rowsAffected > 0;
        }

        // Guardar una evaluación
        public bool GuardarEvaluacionTarea(int tareaId, int estado, int calificacion, string observacion)
        {
            using (var context = new SistemaIntegralGestionEntities())
            {
                var tarea = context.PDA_Tarea.FirstOrDefault(t => t.id == tareaId);
                if (tarea != null)
                {
                    tarea.estado_tarea = estado;
                    tarea.calificacion = calificacion;
                    tarea.observacion = observacion;
                    return context.SaveChanges() > 0;
                }

                return false;
            }
        }

        public Tarea ObtenerTareaPorId(int tareaId)
        {
            using (var context = new SistemaIntegralGestionEntities())
            {
                var tarea = context.PDA_Tarea.FirstOrDefault(t => t.id == tareaId);
                return tarea != null
                    ? new Tarea
                    {
                        Id = tarea.id,
                        DescripcionTarea = tarea.descripcion_tarea,
                        ResponsableId = tarea.responsable_id ?? 0,
                        EstadoTareaId = tarea.estado_tarea ?? 0
                    }
                    : null;
            }
        }
    }
}
