﻿using SIG.BaseDatos;
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
using System.Web.Security;
using System.Web.Mvc;

namespace SIG.Models
{
    public class UsuarioModel
    {
        public iniciarSesion_Result IniciarSesion(UsuarioEmpleado user)
        {
            using (var context = new SistemaIntegralGestionEntities())
            {
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
                    bool correo = context.Empleado.Any(u => u.correo_electronico == user.correo_electronico);
                    if (correo)
                    {
                        return null;
                    }

                    throw new Exception("Usuario no existe.");
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


        public List<SelectListItem> ObtenerDepartamentos()
        {
            using (var context = new SistemaIntegralGestionEntities())
            {
                return context.Emp_Departamento
                    .Select(d => new SelectListItem
                    {
                        Value = d.id.ToString(),
                        Text = d.nombre_departamento
                    })
                    .ToList();
            }
        }

        public UsuarioEmpleado ObtenerPerfilPorId(int id)
        {
            using (var context = new SistemaIntegralGestionEntities())
            {
                return context.Empleado
      .Where(e => e.id == id)
      .Join(context.Emp_Departamento,
          e => e.departamento_id,
          d => d.id,
          (e, d) => new { e, d })
      .Join(context.Emp_Puesto,
          ed => ed.e.puesto_id,
          p => p.id,
          (ed, p) => new { ed.e, ed.d, p })
      .Join(context.Adm_Rol,
          edp => edp.e.rol_id,
          r => r.id,
          (edp, r) => new UsuarioEmpleado
          {
              id_usuario = edp.e.id,
              nombre = edp.e.nombre,
              apellidos = edp.e.apellidos,
              numero_identificacion = edp.e.numero_identificacion,
              fecha_nacimiento = edp.e.fecha_nacimiento,
              direccion = edp.e.direccion,
              telefono = edp.e.telefono,
              correo_electronico = edp.e.correo_electronico,
              usuario = edp.e.usuario,
              nombre_departamento = edp.d.nombre_departamento, 
              nombre_puesto = edp.p.nombre_puesto,
              nombre_rol = r.nombre_rol 
          })
      .FirstOrDefault();

            }
        }

        public string ObtenerDepartamentoPorId(int id)
        {
            using (var context = new SistemaIntegralGestionEntities())
            {
                return context.Emp_Departamento
                    .Where(d => d.id == id)
                    .Select(d => d.nombre_departamento)
                    .FirstOrDefault();
            }
        }

        public string ObtenerPuestoPorId(int id)
        {
            using (var context = new SistemaIntegralGestionEntities())
            {
                return context.Emp_Puesto
                    .Where(p => p.id == id)
                    .Select(p => p.nombre_puesto)
                    .FirstOrDefault();
            }
        }

        public string ObtenerRolPorId(int id)
        {
            using (var context = new SistemaIntegralGestionEntities())
            {
                return context.Adm_Rol
                    .Where(r => r.id == id)
                    .Select(r => r.nombre_rol)
                    .FirstOrDefault();
            }
        }



        public List<SelectListItem> ObtenerListaPuestos()
        {
            using (var context = new SistemaIntegralGestionEntities())
            {
                return context.Emp_Puesto
                    .Select(p => new SelectListItem
                    {
                        Value = p.id.ToString(),
                        Text = p.nombre_puesto 
                    })
                    .ToList();
            }
        }

        public List<SelectListItem> ObtenerListaRoles()
        {
            using (var context = new SistemaIntegralGestionEntities())
            {
                return context.Adm_Rol 
                    .Select(r => new SelectListItem
                    {
                        Value = r.id.ToString(),
                        Text = r.nombre_rol
                    })
                    .ToList();
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
            using (var context = new SistemaIntegralGestionEntities())
            {
                return context.Empleado.ToList();
            }

        }

        public bool EliminarEmpleadoPorId(int id)
        {
            using (var context = new SistemaIntegralGestionEntities())
            {
                var empleado = context.Empleado.FirstOrDefault(e => e.id == id);
                if (empleado != null)
                {

                    empleado.estado_empleado = false;


                    context.SaveChanges();
                    return true;
                }

                return false;
            }
        }

        public bool RestaurarEmpleado(int id)
        {
            using (var context = new SistemaIntegralGestionEntities())
            {
                var empleado = context.Empleado.FirstOrDefault(e => e.id == id);
                if (empleado != null)
                {
                    empleado.estado_empleado = true;
                    context.SaveChanges();
                    return true;
                }
                return false;
            }
        }

        public Empleado ObtenerEmpleadoPorId(int id)
        {
            using (var context = new SistemaIntegralGestionEntities())
            {
                return context.Empleado.FirstOrDefault(e => e.id == id);
            }
        }


        public bool EditarEmpleado(Empleado empleado)
        {
            using (var context = new SistemaIntegralGestionEntities())
            {
                var idParam = new SqlParameter("@Id", empleado.id);
                var nombreParam = new SqlParameter("@Nombre", empleado.nombre);
                var apellidosParam = new SqlParameter("@Apellidos", empleado.apellidos);
                var correoParam = new SqlParameter("@CorreoElectronico", empleado.correo_electronico);
                var telefonoParam = new SqlParameter("@Telefono", empleado.telefono);
                var direccionParam = new SqlParameter("@Direccion", empleado.direccion);
                var fechaNacimientoParam = new SqlParameter("@FechaNacimiento", (object)empleado.fecha_nacimiento ?? DBNull.Value);
                var numeroIdentificacionParam = new SqlParameter("@NumeroIdentificacion", empleado.numero_identificacion);
                var departamentoIdParam = new SqlParameter("@DepartamentoId", empleado.departamento_id);
                var puestoIdParam = new SqlParameter("@PuestoId", empleado.puesto_id);
                var rolIdParam = new SqlParameter("@RolId", empleado.rol_id);

                context.Database.ExecuteSqlCommand("EditarEmpleado @Id, @Nombre, @Apellidos, @CorreoElectronico, @Telefono, @Direccion, @FechaNacimiento, @NumeroIdentificacion, @DepartamentoId, @PuestoId, @RolId",
                    idParam, nombreParam, apellidosParam, correoParam, telefonoParam, direccionParam, fechaNacimientoParam, numeroIdentificacionParam, departamentoIdParam, puestoIdParam, rolIdParam);

                return true;
            }
        }

        public string ObtenerIdentificacion(int idUsuario)
        {
            using (var context = new SistemaIntegralGestionEntities())
            {
                string identificacion = (from x in context.Empleado
                                        where x.id == idUsuario
                                        select x.numero_identificacion).FirstOrDefault();

                return identificacion;
            }
        }

    }
}
