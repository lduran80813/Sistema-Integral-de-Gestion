using SIG.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Net.Mail;
using System.Linq;

namespace SIG.Models
{
    public class NotificacionModel
    {
        private readonly string smtpServer = "smtp.tu-servidor.com"; // Cambia esto con tu servidor SMTP
        private readonly string fromEmail = "notificaciones@tuempresa.com"; // Email desde el cual se enviarán las notificaciones

        public void EnviarNotificacionEntrega(Entrega entrega)
        {
            // Crea el mensaje de correo
            var mailMessage = new MailMessage
            {
                From = new MailAddress(fromEmail),
                Subject = $"Actualización sobre tu Pedido #{entrega.PedidoId}",
                Body = GenerarCuerpoCorreo(entrega),
                IsBodyHtml = true
            };

            mailMessage.To.Add(entrega.CorreoElectronico); // Enviar a la dirección de correo de la entrega

            // Enviar el correo
            using (var smtpClient = new SmtpClient(smtpServer))
            {
                smtpClient.Send(mailMessage);
            }
        }

        private string GenerarCuerpoCorreo(Entrega entrega)
        {
            var cuerpoCorreo = "<h2>Actualización de Pedido</h2>";
            cuerpoCorreo += $"<p>Pedido ID: {entrega.PedidoId}</p>";

            if (entrega.FechaEntrega.HasValue)
            {
                cuerpoCorreo += $"<p>Fecha de Entrega: {entrega.FechaEntrega.Value.ToString("dd/MM/yyyy")}</p>";
            }

            cuerpoCorreo += $"<p>Dirección de Entrega: {entrega.DireccionEntrega}</p>";
            cuerpoCorreo += $"<p>Artículos Entregados: {entrega.ArticulosEntregados}</p>";
            cuerpoCorreo += $"<p>Observaciones Adicionales: {entrega.ObservacionesAdicionales}</p>";
            cuerpoCorreo += $"<p>Estado de la Entrega: {entrega.EstadoEntrega}</p>";
            cuerpoCorreo += $"<p>Nombre del Destinatario: {entrega.NombreDestinatario}</p>";

            return cuerpoCorreo;
        }
    }
}