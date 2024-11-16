using SIG.BaseDatos;
using SIG.Entidades;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Mail;
using System.Net;
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
            new SqlParameter("@FechaEntrega", SqlDbType.DateTime) { Value = entrega.FechaEntrega ?? (object)DBNull.Value },
            new SqlParameter("@DireccionEntrega", SqlDbType.NVarChar, 255) { Value = entrega.DireccionEntrega },
            new SqlParameter("@ArticulosEntregados", SqlDbType.NVarChar, -1) { Value = entrega.ArticulosEntregados },
            new SqlParameter("@ObservacionesAdicionales", SqlDbType.NVarChar, -1) { Value = entrega.ObservacionesAdicionales ?? (object)DBNull.Value },
            new SqlParameter("@EstadoEntrega", SqlDbType.NVarChar, 50) { Value = entrega.EstadoEntrega },
            new SqlParameter("@NombreDestinatario", SqlDbType.NVarChar, 100) { Value = entrega.NombreDestinatario },
            new SqlParameter("@CorreoElectronico", SqlDbType.NVarChar, 100) { Value = entrega.CorreoElectronico },
        };

                var rowsAffected = context.Database.ExecuteSqlCommand(
                    "EXEC RegistrarEntrega @PedidoId, @FechaEntrega, @DireccionEntrega, @ArticulosEntregados, @ObservacionesAdicionales, @EstadoEntrega, @NombreDestinatario, @CorreoElectronico",
                    parameters.ToArray());

                return rowsAffected > 0;
            }
        }


        public List<Entrega> ListarEntregas()
        {
            using (var context = new SistemaIntegralGestionEntities())
            {
                return (from e in context.Entregas
                        join vp in context.Venta_Factura on e.pedido_id equals vp.id 
                        select new Entrega
                        {
                            PedidoId = vp.id,
                            FechaEntrega = e.fecha_entrega,
                            DireccionEntrega = e.direccion_entrega,
                            ArticulosEntregados = e.articulos_entregados,
                            ObservacionesAdicionales = e.observaciones_adicionales,
                            EstadoEntrega = e.EstadoEntrega,
                            NombreDestinatario = e.NombreDestinatario,
                            CorreoElectronico = e.CorreoElectronico
                        }).ToList();
            }
        }

        public Entrega ObtenerEntregaPorId(int id)
        {
            using (var context = new SistemaIntegralGestionEntities())
            {
                var entrega = context.Entregas
                                     .FirstOrDefault(e => e.pedido_id == id);

                return entrega != null ? new Entrega
                {
                    PedidoId = entrega.pedido_id,
                    FechaEntrega = entrega.fecha_entrega,
                    DireccionEntrega = entrega.direccion_entrega,
                    ArticulosEntregados = entrega.articulos_entregados,
                    ObservacionesAdicionales = entrega.observaciones_adicionales,
                    EstadoEntrega = entrega.EstadoEntrega,
                    NombreDestinatario = entrega.NombreDestinatario,
                    CorreoElectronico= entrega.CorreoElectronico
                } : null;
            }
        }

        public bool ActualizarEntrega(Entrega entrega)
        {
            using (var context = new SistemaIntegralGestionEntities())
            {
                var parameters = new List<SqlParameter>
        {
            new SqlParameter("@PedidoId", SqlDbType.Int) { Value = entrega.PedidoId },
            new SqlParameter("@FechaEntrega", SqlDbType.DateTime) { Value = entrega.FechaEntrega ?? (object)DBNull.Value },
            new SqlParameter("@DireccionEntrega", SqlDbType.NVarChar, 255) { Value = entrega.DireccionEntrega },
            new SqlParameter("@ArticulosEntregados", SqlDbType.NVarChar, -1) { Value = entrega.ArticulosEntregados },
            new SqlParameter("@ObservacionesAdicionales", SqlDbType.NVarChar, -1) { Value = entrega.ObservacionesAdicionales ?? (object)DBNull.Value },
            new SqlParameter("@EstadoEntrega", SqlDbType.NVarChar, 50) { Value = entrega.EstadoEntrega },
            new SqlParameter("@NombreDestinatario", SqlDbType.NVarChar, 100) { Value = entrega.NombreDestinatario },
            new SqlParameter("@CorreoElectronico", SqlDbType.NVarChar, 100) { Value = entrega.CorreoElectronico }
        };

                var rowsAffected = context.Database.ExecuteSqlCommand(
                    "EXEC ActualizarEntrega @PedidoId, @FechaEntrega, @DireccionEntrega, @ArticulosEntregados, @ObservacionesAdicionales, @EstadoEntrega, @NombreDestinatario, @CorreoElectronico",
                    parameters.ToArray());

                return rowsAffected > 0;
            }
        }

        public void EnviarNotificacionCorreo(string destinatario, string asunto, string mensajeHtml)
        {
            string cuenta = "lthx05@gmail.com";
            string contrasenna = "ebni gxco iflo mdkc";
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(cuenta, contrasenna),
                EnableSsl = true,
            };


            var correo = new MailMessage
            {
                From = new MailAddress("lthx05@gmail.com", "Sistema de Entregas"),
                Subject = asunto,
                Body = mensajeHtml,
                IsBodyHtml = true
            };
            correo.To.Add(destinatario);

            smtpClient.Send(correo);
        }

        public Entregas ObtenerPorId(int pedidoId)
        {
            using (var context = new SistemaIntegralGestionEntities())
            {
                return context.Entregas.FirstOrDefault(e => e.pedido_id == pedidoId);
            }
        }


    }



}