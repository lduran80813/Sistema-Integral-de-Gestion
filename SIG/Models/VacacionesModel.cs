using SIG.BaseDatos;
using SIG.Entidades;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;

namespace SIG.Models
{
    public class VacacionesModel
    {
        public bool SolicitarVacaciones(int empleadoId, DateTime fechaInicio, DateTime fechaFin, int diasSolicitados, string observaciones, string motivoRechazo)
        {
            try
            {
                using (var context = new SistemaIntegralGestionEntities())
                {
                    // Insertar la solicitud de vacaciones en la base de datos
                    var solicitud = new SolicitudesVacaciones
                    {
                        empleado_id = empleadoId,
                        fecha_inicio = fechaInicio,
                        fecha_fin = fechaFin,
                        dias_solicitados = diasSolicitados,
                        observaciones = observaciones,
                        estado = "Pendiente", // Asumimos que la solicitud se crea con estado 'Pendiente'
                        fecha_solicitud = DateTime.Now,
                        motivo_rechazo = motivoRechazo,
                        aprobado_por = null,
                        fecha_aprobacion = null
                    };

                    context.SolicitudesVacaciones.Add(solicitud);
                    context.SaveChanges();

                    return true; // La solicitud se guardó correctamente
                }
            }
            catch (Exception ex)
            {
                // Log o manejo del error
                return false;
            }
        }


        public void AprobarRechazarVacaciones(int solicitudId, string estado, int administradorId, string motivoRechazo = null)
        {
            using (var context = new SistemaIntegralGestionEntities())
            {
                // Configurar los parámetros del procedimiento almacenado
                var solicitudIdParam = new SqlParameter("@solicitud_id", solicitudId);
                var estadoParam = new SqlParameter("@estado", estado);
                var administradorIdParam = new SqlParameter("@aprobado_por", administradorId);
                var motivoRechazoParam = new SqlParameter("@motivo_rechazo",
                    string.IsNullOrEmpty(motivoRechazo) ? (object)DBNull.Value : motivoRechazo);

                // Llamar al procedimiento almacenado para actualizar el estado de la solicitud
                context.Database.ExecuteSqlCommand(
                    "EXEC aprobarrechazarvacaciones @solicitud_id, @estado, @aprobado_por, @motivo_rechazo",
                    solicitudIdParam, estadoParam, administradorIdParam, motivoRechazoParam);
            
        }
        }

        public bool AprobarRechazarVac(int solicitudId, string estado, int administradorId, string motivoRechazo)
        {
            try
            {
                using (var context = new SistemaIntegralGestionEntities())
                {
                    // Configurar los parámetros del procedimiento almacenado
                    var solicitudIdParam = new SqlParameter("@SolicitudId", solicitudId);
                    var estadoParam = new SqlParameter("@Estado", estado);
                    var administradorIdParam = new SqlParameter("@AprobadoPor", administradorId);
                    var motivoRechazoParam = new SqlParameter("@MotivoRechazo",
                        string.IsNullOrEmpty(motivoRechazo) ? (object)DBNull.Value : motivoRechazo);

                    // Llamar al procedimiento almacenado para aprobar o rechazar la solicitud de vacaciones
                    context.Database.ExecuteSqlCommand(
                        "EXEC AprobarRechazarVacaciones @SolicitudId, @Estado, @AprobadoPor, @MotivoRechazo",
                        solicitudIdParam, estadoParam, administradorIdParam, motivoRechazoParam);
                }

                return true; // Retorna verdadero si el proceso fue exitoso
            }
            catch (Exception ex)
            {
                // Manejo de errores: Log o manejo adecuado
                Console.WriteLine($"Error al procesar la solicitud: {ex.Message}");
                return false; // Retorna falso si ocurre algún error
            }
        }
    }
}