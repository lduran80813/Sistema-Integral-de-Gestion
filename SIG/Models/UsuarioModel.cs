using SIG.BaseDatos;
using SIG.Entidades;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace SIG.Models
{
    public class UsuarioModel
    {
        public IniciarSesion_Result IniciarSesion(UsuarioEmpleado user)
        {
            using (var context = new SistemaIntegralGestionEntities())
            {
                var idOutput = new SqlParameter("@Id", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };

                var loginSuccessOutput = new SqlParameter("@LoginSuccess", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };

                context.Database.ExecuteSqlCommand(
                    "EXEC IniciarSesion @Usuario, @Contrasena, @Id OUTPUT, @LoginSuccess OUTPUT",
                    new SqlParameter("@Usuario", user.usuario),
                    new SqlParameter("@Contrasena", user.contrasena),
                    idOutput,
                    loginSuccessOutput
                );


                if (loginSuccessOutput.Value != DBNull.Value && Convert.ToInt32(loginSuccessOutput.Value) == 1)
                {
                    return new IniciarSesion_Result
                    {
                        id = (int)idOutput.Value,
                        usuario = user.usuario,
                        rol_id = 1,
                        empleado_id = 0
                    };
                }
                else
                {
                    throw new UsuarioNoEncontradoException();
                }
            }
        }


        public class UsuarioNoEncontradoException : Exception
        {
            public UsuarioNoEncontradoException() : base("Credenciales incorrectas. Verifique su usuario o contraseña.")
            {
            }
        }

        public bool CambiarContrasenna(string usuario, string contrasennaTemporal)
        {
            using (var context = new SistemaIntegralGestionEntities())
            {

                var resultadoParameter = new SqlParameter("@Resultado", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };


                var usuarioParameter = new SqlParameter("@Usuario", usuario);
                var contrasenaParameter = new SqlParameter("@ContrasenaTemporal", contrasennaTemporal);


                context.Database.ExecuteSqlCommand(
                    "EXEC CambiarContrasenna @Usuario, @ContrasenaTemporal, @Resultado OUTPUT",
                    usuarioParameter,
                    contrasenaParameter,
                    resultadoParameter
                );

           
                return (int)resultadoParameter.Value == 1; 
            }
        }

        public void EnviarCorreo(string destino, string asunto, string contenido)
        {
            // Tu dirección de correo de Gmail y la contraseña (o contraseña de aplicación)
            string cuenta = "lthx05@gmail.com"; // Reemplaza con tu correo
            string contrasenna = "ebni gxco iflo mdkc"; // Usa la contraseña de aplicación si es necesario

            try
            {
                using (MailMessage message = new MailMessage())
                {
                    message.From = new MailAddress(cuenta);
                    message.To.Add(new MailAddress(destino));
                    message.Subject = asunto;
                    message.Body = contenido;
                    message.IsBodyHtml = true;

                    using (SmtpClient client = new SmtpClient("smtp.gmail.com", 587))
                    {
                        client.Credentials = new NetworkCredential(cuenta, contrasenna);
                        client.EnableSsl = true; // Debe estar habilitado
                        client.Send(message);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al enviar el correo: " + ex.Message);
            }
        }


        public string CreatePassword()
        {
            int length = 6;
            const string valid = "ABCDEFGHIJKLMNOPQRSTUVWXYZ123456789";
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            while (0 < length--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            return res.ToString();
        }

        public ValidarCorreo_Result ValidarCorreo(string correo)
        {
            using (var context = new SistemaIntegralGestionEntities())
            {
                return (context.ValidarCorreo(correo).FirstOrDefault());
            }
        }
    }
}