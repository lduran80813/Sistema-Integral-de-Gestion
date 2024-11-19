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

        public bool ExisteNumeroPedido(int? pedidoId)
        {
            if (!pedidoId.HasValue)
                return false; 

            using (var context = new SistemaIntegralGestionEntities())
            {
                return context.Venta_Factura.Any(p => p.id == pedidoId.Value);
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


        public bool CancelarEntrega(int pedidoId, string correoUsuario, string correoAdmin)
        {
            using (var context = new SistemaIntegralGestionEntities())
            {
                var parameters = new List<SqlParameter>
        {
            new SqlParameter("@PedidoId", SqlDbType.Int) { Value = pedidoId }
        };


                var rowsAffected = context.Database.ExecuteSqlCommand(
                    "EXEC CancelarEntrega @PedidoId",
                    parameters.ToArray());

                if (rowsAffected > 0)
                {

                    var entrega = ObtenerPorId(pedidoId);
                    if (entrega != null)
                    {

                        string cuerpoCorreo = $@"
                    <p>La entrega con los siguientes detalles ha sido cancelada:</p>
                    <ul>
                        <li><strong>Número de Pedido:</strong> {entrega.pedido_id}</li>
                        <li><strong>Fecha de Entrega:</strong> {entrega.fecha_entrega?.ToString("dd/MM/yyyy") ?? "N/A"}</li>
                        <li><strong>Dirección de Entrega:</strong> {entrega.direccion_entrega}</li>
                        <li><strong>Nombre del Destinatario:</strong> {entrega.NombreDestinatario}</li>
                        <li><strong>Estado de la Entrega:</strong> Cancelado</li>
                    </ul>";


                        EnviarNotificacionCorreo(correoUsuario, "Cancelación de Entrega", cuerpoCorreo);


                        EnviarNotificacionCorreo(correoAdmin, "Notificación de Cancelación", cuerpoCorreo);
                    }

                    return true;
                }

                return false;
            }
        }



    }



}