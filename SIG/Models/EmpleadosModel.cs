using SIG.BaseDatos;
using SIG.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SIG.Models
{
    public class EmpleadosModel
    {

        public List<Entidades.EmpCapacitacion> ListarCapacitaciones()
        {
            using (var context = new SistemaIntegralGestionEntities())
            {
                return (from x in context.Emp_Capacitacion
                        select new Entidades.EmpCapacitacion
                        {
                            id = x.id,
                            nombre_capacitacion = x.nombre_capacitacion,
                            descripcion_capacitacion = x.descripcion_capacitacion,
                            fecha_inicio = (DateTime)x.fecha_inicio,
                            fecha_finalizacion = (DateTime)x.fecha_finalizacion,
                            ResponsableId = x.ResponsableId ?? 0
                        }).ToList();
            }
        }

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

        public bool InsertarCapacitacion(Entidades.EmpCapacitacion capacitacion)
        {
            var rowsAffected = 0;

            using (var context = new SistemaIntegralGestionEntities())
            {
                int idSesion = int.Parse(HttpContext.Current.Session["IdUsuario"].ToString());

                var tCapacitacion = new SIG.BaseDatos.Emp_Capacitacion
                {
                    id = idSesion,
                    nombre_capacitacion = capacitacion.nombre_capacitacion,
                    descripcion_capacitacion = capacitacion.descripcion_capacitacion,
                    fecha_inicio = capacitacion.fecha_inicio,
                    fecha_finalizacion = capacitacion.fecha_finalizacion,
                    ResponsableId = capacitacion.ResponsableId
                };

                context.Emp_Capacitacion.Add(tCapacitacion);
                rowsAffected = context.SaveChanges();

                //if (rowsAffected > 0 && capacitacion.Participantes != null)
                //{
                //    foreach (var participante in capacitacion.Participantes)
                //    {
                //        var tParticipante = new SIG.BaseDatos.Emp_Capacitacion_Participantes
                //        {
                //            participante_id = participante.participante_id,
                //            capacitacion_id = tCapacitacion.id
                //        };

                //        context.Emp_Capacitacion_Participantes.Add(tParticipante);
                //    }

                //    rowsAffected = context.SaveChanges();
                //}
            }

            return rowsAffected > 0;
        }

        public Entidades.EmpCapacitacion VerCapacitacion(int idCapacitacion)
        {
            using (var context = new SistemaIntegralGestionEntities())
            {

                var capacitacion = (from x in context.Emp_Capacitacion
                            where x.id == idCapacitacion
                            select new Entidades.EmpCapacitacion
                            {
                                id = x.id,
                                nombre_capacitacion = x.nombre_capacitacion,
                                descripcion_capacitacion = x.descripcion_capacitacion,
                                fecha_inicio = (DateTime)x.fecha_inicio,
                                fecha_finalizacion = (DateTime)x.fecha_finalizacion,
                                ResponsableId = x.ResponsableId ?? 0
                            }).FirstOrDefault();

                if (capacitacion == null)
                {
                    return null;
                }

                var participantes = (from t in context.Emp_Capacitacion_Participantes
                              where t.capacitacion_id == idCapacitacion
                              select new
                              {
                                  t.id,
                                  t.capacitacion_id,
                                  t.participante_id,
                                  t.calificacion,
                                  t.retroalimentacion
                              }).ToList();

                capacitacion.Participantes = participantes.Select(t => new EmpCapacitacionParticipantes
                {
                    id = t.id,
                    capacitacion_id = (int)t.capacitacion_id,
                    participante_id = (int)t.participante_id,
                    calificacion = (int)t.calificacion,
                    retroalimentacion = t.retroalimentacion
                }).ToList();

                var participantesIds = capacitacion.Participantes.Select(t => t.participante_id).Distinct().ToList();

                var empleados = context.Empleado
                    .Where(e => participantesIds.Contains(e.id))
                    .ToList();

                foreach (var partipante in capacitacion.Participantes)
                {
                    var empleado = empleados.FirstOrDefault(e => e.id == partipante.participante_id);
                    if (empleado != null)
                    {
                        partipante.Participante = new PDAEmpleado
                        {
                            Id = empleado.id,
                            Nombre = $"{empleado.nombre} {empleado.apellidos}"
                        };
                    }

                }

                return capacitacion;
            }
        }

        // Actualizar un Plan de Acción existente
        public bool ActualizarCapacitacion(Entidades.EmpCapacitacion capacitacion)
        {
            var rowsAffected = 0;

            using (var context = new SistemaIntegralGestionEntities())
            {

                var capacitacionExistente = context.Emp_Capacitacion.FirstOrDefault(p => p.id == capacitacion.id);
                if (capacitacionExistente != null)
                {

                    capacitacionExistente.nombre_capacitacion = capacitacion.nombre_capacitacion;
                    capacitacionExistente.descripcion_capacitacion = capacitacion.descripcion_capacitacion;
                    capacitacionExistente.fecha_inicio = capacitacion.fecha_inicio;
                    capacitacionExistente.fecha_finalizacion = capacitacion.fecha_finalizacion;
                    capacitacionExistente.ResponsableId = capacitacion.ResponsableId;

                    rowsAffected = context.SaveChanges();
                }
            }

            return rowsAffected > 0;
        }

    }
}