using SIG.BaseDatos;
using SIG.Entidades;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SIG.Models
{
    public class EntregasModel
    {
        public bool RegistrarEntrega(Entrega entrega)
        {
            using (var context = new SistemaIntegralGestionEntities())
            {
                var parameters = new List<SqlParameter>
        {
            new SqlParameter("@PedidoId", SqlDbType.Int) { Value = entrega.PedidoId },
            new SqlParameter("@FechaEntrega", SqlDbType.DateTime) { Value = entrega.FechaEntrega },
            new SqlParameter("@DireccionEntrega", SqlDbType.NVarChar, 255) { Value = entrega.DireccionEntrega },
            new SqlParameter("@ArticulosEntregados", SqlDbType.NVarChar, -1) { Value = entrega.ArticulosEntregados },
            new SqlParameter("@ObservacionesAdicionales", SqlDbType.NVarChar, -1) { Value = entrega.ObservacionesAdicionales }
        };

                var rowsAffected = context.Database.ExecuteSqlCommand(
                    "EXEC RegistrarEntrega @PedidoId, @FechaEntrega, @DireccionEntrega, @ArticulosEntregados, @ObservacionesAdicionales",
                    parameters.ToArray());

                return rowsAffected > 0; 
            }
        }


        public List<Entrega> ObtenerTodasLasEntregas()
        {
            using (var context = new SistemaIntegralGestionEntities())
            {
                var entregas = context.Database.SqlQuery<Entrega>(
                    "EXEC ObtenerTodasLasEntregas"
                ).ToList();

                return entregas;
            }
        }



    }



}