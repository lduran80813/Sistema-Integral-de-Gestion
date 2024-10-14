using SIG.BaseDatos;
using SIG.Entidades;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace SIG.Models
{
    public class UsuarioModel
    {
        public iniciarSesion_Result IniciarSesion(UsuarioEmpleado user)
        {
            using (var context = new SistemaIntegralGestionEntities())
            {
                // Parámetros de salida
                var idParam = new SqlParameter
                {
                    ParameterName = "@Id",
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Output
                };
                var loginSuccessParam = new SqlParameter
                {
                    ParameterName = "@LoginSuccess",
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Output
                };
                var rolParam = new SqlParameter
                {
                    ParameterName = "@Rol",
                    SqlDbType = SqlDbType.Int, 
                    Direction = ParameterDirection.Output
                };
                var usuarioParam = new SqlParameter
                {
                    ParameterName = "@Usuario",
                    SqlDbType = SqlDbType.NVarChar,
                    Size = 100,
                    Direction = ParameterDirection.Output
                };
                context.Database.ExecuteSqlCommand(
                    "EXEC iniciarSesion @correo_electronico, @Contrasena, @Id OUTPUT, @LoginSuccess OUTPUT, @Rol OUTPUT, @Usuario OUTPUT",
                    new SqlParameter("@correo_electronico", user.correo_electronico),
                    new SqlParameter("@Contrasena", user.contrasena), 
                    idParam,
                    loginSuccessParam,
                    rolParam,
                    usuarioParam
                );
                if (loginSuccessParam.Value != DBNull.Value && (int)loginSuccessParam.Value == 1)
                {
                    return new iniciarSesion_Result
                    {
                        id = (int)idParam.Value,
                        nombre = (string)usuarioParam.Value,
                        rol_id = (int)rolParam.Value
                    };
                }
                else
                {
                    throw new Exception("Usuario o contraseña incorrectos.");
                }
            }
        }
        public bool CambiarContrasenna(int id, string contrasennaTemporal)
        {
            using (var context = new SistemaIntegralGestionEntities())
            {

                var usuarioParameter = new SqlParameter("@Id", id) { Direction = ParameterDirection.InputOutput }; 
                var contrasenaParameter = new SqlParameter("@nuevaContrasena", contrasennaTemporal);

                var result = context.Database.ExecuteSqlCommand(
                    "EXEC CambiarContrasenna @id OUTPUT, @nuevaContrasena",
                    usuarioParameter,
                    contrasenaParameter
                );

                return (int)usuarioParameter.Value == id;
            }
        }



        public ValidarCorreo_Result ValidarCorreo(string correo)
        {
            using (var context = new SistemaIntegralGestionEntities())
            {
                
                return (context.ValidarCorreo(correo).FirstOrDefault());
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

        public void EnviarCorreo(string destino, string asunto, string contenido)
        {

            string cuenta = "lthx05@gmail.com";
            string contrasenna = "ebni gxco iflo mdkc";

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
                        client.EnableSsl = true;
                        client.Send(message);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al enviar el correo: " + ex.Message);
            }
        }


        public bool RegistrarUsuario(UsuarioEmpleado user)
        {
            try
            {
                using (var context = new SistemaIntegralGestionEntities())
                {
                    var parameters = new List<SqlParameter>
            {
                new SqlParameter("@Nombre", SqlDbType.NVarChar) { Value = user.nombre },
                new SqlParameter("@Apellidos", SqlDbType.NVarChar) { Value = user.apellidos },
                new SqlParameter("@NumeroIdentificacion", SqlDbType.NVarChar) { Value = user.numero_identificacion },
                new SqlParameter("@FechaNacimiento", SqlDbType.Date) { Value = user.fecha_nacimiento },
                new SqlParameter("@Direccion", SqlDbType.NVarChar) { Value = user.direccion },
                new SqlParameter("@Telefono", SqlDbType.NVarChar) { Value = user.telefono },
                new SqlParameter("@CorreoElectronico", SqlDbType.NVarChar) { Value = user.correo_electronico },
                new SqlParameter("@DepartamentoID", SqlDbType.Int) { Value = 1 }, 
                new SqlParameter("@PuestoID", SqlDbType.Int) { Value = 1 },   
                new SqlParameter("@RolID", SqlDbType.Int) { Value = 1 },    
                new SqlParameter("@EstadoEmpleado", SqlDbType.Bit) { Value = user.estado_usuario },
                new SqlParameter("@Usuario", SqlDbType.NVarChar) { Value = user.usuario },
                new SqlParameter("@Contrasena", SqlDbType.NVarChar) { Value = user.contrasena } 
            };

                    var rowsAffected = context.Database.ExecuteSqlCommand(
                        "EXEC registrarEmpleado @Nombre, @Apellidos, @NumeroIdentificacion, @FechaNacimiento, @Direccion, @Telefono, @CorreoElectronico, @DepartamentoID, @PuestoID, @RolID, @EstadoEmpleado, @Usuario, @Contrasena",
                        parameters.ToArray());

                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al registrar el usuario", ex);
            }
        }






        public bool consultaCedulaCorreo(UsuarioEmpleado user)
        {
            var rowsAffected = 0;

            using (var context = new SistemaIntegralGestionEntities())
            {
                rowsAffected = (from x in context.Empleado
                                where x.numero_identificacion == user.numero_identificacion || x.correo_electronico == user.correo_electronico
                                select x).Count();
            }
            return (rowsAffected > 0 ? true : false);
        }


        public List<Empleado> ListarEmpleados() 
        {
            using(var context = new SistemaIntegralGestionEntities())
            {
                return context.Empleado.ToList();
            }
        
        }




    }
}
