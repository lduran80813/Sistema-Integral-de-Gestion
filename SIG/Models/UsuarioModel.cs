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
                    SqlDbType = SqlDbType.Int, // Cambiado a INT
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
                    new SqlParameter("@Contrasena", user.contrasena), // Usar directamente la contraseña sin hashear
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

        public class UsuarioNoEncontradoException : Exception
        {
            public UsuarioNoEncontradoException() : base("El usuario no existe en la base de datos.")
            {
            }
        }
        public bool CambiarContrasenna(int id, string contrasennaTemporal)
        {
            using (var context = new SistemaIntegralGestionEntities())
            {
                // Parámetros de entrada
                var usuarioParameter = new SqlParameter("@Id", id) { Direction = ParameterDirection.InputOutput }; // Salida
                var contrasenaParameter = new SqlParameter("@nuevaContrasena", contrasennaTemporal);  // Mantenlo como NVARCHAR

                // Ejecutar el procedimiento almacenado
                var result = context.Database.ExecuteSqlCommand(
                    "EXEC CambiarContrasenna @id OUTPUT, @nuevaContrasena",
                    usuarioParameter,
                    contrasenaParameter
                );

                // Si se actualizó al menos una fila, el cambio fue exitoso
                return (int)usuarioParameter.Value == id;
            }
        }





    }
}
