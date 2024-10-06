using SIG.BaseDatos;
using SIG.Entidades;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
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

                if ((int)loginSuccessOutput.Value == 1)
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
                    return null;
                }
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




    }
}