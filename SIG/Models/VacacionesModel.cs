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
        public string SolicitarVacaciones(string correoEmpleado, DateTime fechaInicio, DateTime fechaFin, string observaciones)
        {
            using (var context = new SistemaIntegralGestionEntities())
            {
                // Calcular la cantidad de días solicitados
                int diasSolicitados = (fechaFin - fechaInicio).Days + 1;

                // Configurar los parámetros del procedimiento almacenado
                var correoEmpleadoParam = new SqlParameter("@CorreoEmpleado", correoEmpleado);
                var fechaInicioParam = new SqlParameter("@FechaInicio", fechaInicio);
                var fechaFinParam = new SqlParameter("@FechaFin", fechaFin);
                var diasSolicitadosParam = new SqlParameter("@DiasSolicitados", diasSolicitados);
                var observacionesParam = new SqlParameter("@Observaciones", observaciones ?? (object)DBNull.Value);

                // Configurar el parámetro de salida para el resultado
                var resultadoParam = new SqlParameter("@Resultado", SqlDbType.NVarChar, 50)
                {
                    Direction = ParameterDirection.Output
                };

                // Llamar al procedimiento almacenado
                context.Database.ExecuteSqlCommand(
                    "EXEC RegistrarSolicitudVacaciones @CorreoEmpleado, @FechaInicio, @FechaFin, @DiasSolicitados, @Observaciones, @Resultado OUTPUT",
                    correoEmpleadoParam, fechaInicioParam, fechaFinParam, diasSolicitadosParam, observacionesParam, resultadoParam);

                // Retornar el resultado
                return resultadoParam.Value.ToString();
            }
        }

        public void AprobarRechazarVacaciones(int solicitudId, string estado, int administradorId, string motivoRechazo = null)
        {
            using (var context = new SistemaIntegralGestionEntities())
            {
                // Configurar los parámetros del procedimiento almacenado
                var solicitudIdParam = new SqlParameter("@SolicitudId", solicitudId);
                var estadoParam = new SqlParameter("@Estado", estado);
                var administradorIdParam = new SqlParameter("@AprobadoPor", administradorId);
                var motivoRechazoParam = new SqlParameter("@MotivoRechazo", motivoRechazo ?? (object)DBNull.Value);

                // Llamar al procedimiento almacenado para actualizar el estado
                context.Database.ExecuteSqlCommand(
                    "EXEC AprobarRechazarVacaciones @SolicitudId, @Estado, @AprobadoPor, @MotivoRechazo",
                    solicitudIdParam, estadoParam, administradorIdParam, motivoRechazoParam);
            }
        }
    }
}