using SIG.BaseDatos;
using SIG.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SIG.Models
{
    public class VacacionesModel
    {
        public ResultadoOperativo SolicitarVacaciones(Vacaciones modelo)
        {
            using (var context = new SistemaIntegralGestionEntities())
            {
                // Verificar días disponibles
                var diasDisponibles = context.DiasVacacionesDisponibles
                    .FirstOrDefault(x => x.empleado_id == modelo.EmpleadoId)?.dias_disponibles ?? 0;

                int diasSolicitados = (modelo.FechaFin - modelo.FechaInicio).Days + 1;

                if (diasSolicitados > diasDisponibles)
                {
                    return new ResultadoOperativo
                    {
                        Exitoso = false,
                        MensajeError = "No tienes suficientes días disponibles."
                    };
                }

                // Crear la solicitud
                var solicitud = new Emp_Vacaciones
                {
                    empleado_id = modelo.EmpleadoId,
                    fecha_inicio = modelo.FechaInicio,
                    fecha_fin = modelo.FechaFin,
                    supervisor_id = modelo.SupervisorId,
                    estado = "Pendiente",
                    fecha_solicitud = DateTime.Now
                };

                context.Emp_Vacaciones.Add(solicitud);
                context.SaveChanges();

                return new ResultadoOperativo { Exitoso = true };
            }
        }

        // Obtener historial de solicitudes del empleado
        public List<Vacaciones> ObtenerHistorial(int empleadoId)
        {
            using (var context = new SistemaIntegralGestionEntities())
            {
                return (from x in context.Emp_Vacaciones
                        where x.empleado_id == empleadoId
                        select new Vacaciones
                        {
                            SolicitudId = x.id,
                            EmpleadoId = x.empleado_id.GetValueOrDefault(),
                            FechaInicio = x.fecha_inicio.GetValueOrDefault(),
                            FechaFin = x.fecha_fin.GetValueOrDefault(),
                            Estado = x.estado,
                            Comentario = x.comentario,
                            FechaSolicitud = x.fecha_solicitud,
                            FechaRespuesta = x.fecha_respuesta
                        }).ToList();
            }
        }

        // Obtener solicitudes pendientes para el supervisor
        public List<Vacaciones> ObtenerPendientes(int supervisorId)
        {
            using (var context = new SistemaIntegralGestionEntities())
            {
                return (from x in context.Emp_Vacaciones
                        where x.supervisor_id == supervisorId && x.estado == "Pendiente"
                        select new Vacaciones
                        {
                            SolicitudId = x.id,
                            EmpleadoId = x.empleado_id.GetValueOrDefault(),
                            FechaInicio = x.fecha_inicio.GetValueOrDefault(),
                            FechaFin = x.fecha_fin.GetValueOrDefault(),
                            Estado = x.estado,
                            FechaSolicitud = x.fecha_solicitud
                        }).ToList();
            }
        }

        // Responder a una solicitud (aprobar/rechazar)
        public ResultadoOperativo ResponderSolicitud(int solicitudId, string estado, string comentario)
        {
            using (var context = new SistemaIntegralGestionEntities())
            {
                var solicitud = context.Emp_Vacaciones.FirstOrDefault(x => x.id == solicitudId);
                if (solicitud == null)
                {
                    return new ResultadoOperativo
                    {
                        Exitoso = false,
                        MensajeError = "Solicitud no encontrada."
                    };
                }

                solicitud.estado = estado;
                solicitud.comentario = comentario;
                solicitud.fecha_respuesta = DateTime.Now;

                context.SaveChanges();

                return new ResultadoOperativo { Exitoso = true };
            }
        }
    }
}