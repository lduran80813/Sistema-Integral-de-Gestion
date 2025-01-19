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

                    var solicitud = new SolicitudesVacaciones
                    {
                        empleado_id = empleadoId,
                        fecha_inicio = fechaInicio,
                        fecha_fin = fechaFin,
                        dias_solicitados = diasSolicitados,
                        observaciones = observaciones,
                        estado = "Pendiente", 
                        fecha_solicitud = DateTime.Now,
                        motivo_rechazo = motivoRechazo,
                        aprobado_por = null,
                        fecha_aprobacion = null
                    };

                    context.SolicitudesVacaciones.Add(solicitud);
                    context.SaveChanges();

                    return true; 
                }
            }
            catch (Exception ex)
            {

                return false;
            }
        }

        public List<Vacaciones> ListarTodasVacaciones()
        {
            using (var context = new SistemaIntegralGestionEntities())
            {
                var solicitudes = context.Database.SqlQuery<Vacaciones>(
                    "EXEC sp_ListarTodasVacaciones").ToList();

                return solicitudes;
            }
        }



    }
}