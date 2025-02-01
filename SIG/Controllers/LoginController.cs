using SIG.BaseDatos;
using SIG.Entidades;
using SIG.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using System.Web.ApplicationServices;
using System.Web.Services.Description;


namespace SIG.Controllers
{
    public class LoginController : Controller
    {
        UsuarioModel usuarioM = new UsuarioModel();

        public ActionResult Login()
        {
            return View();
        }


        [HttpPost]
        public ActionResult Login(UsuarioEmpleado user)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errores = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();
                    ViewBag.msj = string.Join(", ", errores);
                    return View(user);
                }

                var respuesta = usuarioM.IniciarSesion(user);

                if (respuesta != null)
                {
                    Session["Usuario"] = respuesta.nombre;
                    Session["IdUsuario"] = respuesta.id;
                    Session["RolUsuario"] = respuesta.rol_id;


                    Session["NombreRolUsuario"] = respuesta.rol_id.HasValue
                        ? usuarioM.ObtenerRolPorId(respuesta.rol_id.Value)
                        : "No asignado";

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.msj = "Credenciales incorrectas.";
                    return View(user);
                }
            }
            catch (Exception ex)
            {
                ViewBag.msj = "Ocurrió un error al intentar iniciar sesión. Inténtalo de nuevo más tarde.";
                return View(user);
            }
        }


        public ActionResult VerPerfil()
        {

            var idUsuario = Session["IdUsuario"] as int?;

            if (idUsuario.HasValue)
            {

                var perfil = usuarioM.ObtenerPerfilPorId(idUsuario.Value);


                return View(perfil);
            }
            else
            {

                return RedirectToAction("Login", "Home");
            }
        }



        [HttpGet]
        public ActionResult CambiarContrasenna()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login", "Login");
        }
        [HttpPost]
        public ActionResult CambiarContrasenna(UsuarioEmpleado u)
        {
            if (u.contrasena != u.contrasenaConfirmar)
            {
                ViewBag.msj = "Las contraseñas no coinciden.";
                return View();
            }

            if (Session["IdUsuario"] == null)
            {
                ViewBag.msj = "Sesión expirada. Por favor, inicie sesión nuevamente.";
                return RedirectToAction("Login", "Login");
            }

            int usuarioId = Convert.ToInt32(Session["IdUsuario"]);

            try
            {
                var resultado = usuarioM.CambiarContrasenna(usuarioId, u.contrasena);

                if (resultado)
                {
                    ViewBag.msj = "La contraseña se ha actualizado exitosamente.";
                    return RedirectToAction("Logout", "Login");
                }
                else
                {
                    ViewBag.msj = $"No se pudo actualizar la contraseña para el usuario con ID {usuarioId}.";
                    return View();
                }
            }
            catch (Exception ex)
            {
                ViewBag.msj = $"Ocurrió un error al intentar cambiar la contraseña: {ex.Message}";
                return View();
            }
        }


        [HttpPost]
        public ActionResult EliminarEmpleado(int id)
        {

            bool resultado = usuarioM.EliminarEmpleadoPorId(id);

            if (resultado)
            {
                return RedirectToAction("ListarUsuarios");
            }

            return View("Error");
        }


        [HttpPost]
        public ActionResult RestaurarEmpleado(int id)
        {
            bool resultado = usuarioM.RestaurarEmpleado(id); 

            if (resultado)
            {
                TempData["Mensaje"] = "Empleado restaurado exitosamente.";
            }
            else
            {
                TempData["Mensaje"] = "No se pudo restaurar el empleado.";
            }

            return RedirectToAction("ListarUsuarios"); 
        }

        [HttpGet]
        public ActionResult RecuperarAcceso()
        {
            return View();
        }

        [HttpPost]
        public ActionResult RecuperarAcceso(UsuarioEmpleado u)
        {
            if (u == null || string.IsNullOrWhiteSpace(u.correo_electronico))
            {
                ViewBag.msj = "El objeto usuario no está inicializado o el correo electrónico no es válido.";
                return View();
            }
            var respuesta = usuarioM.ValidarCorreo(u.correo_electronico);

            if (respuesta == null)
            {
                ViewBag.msj = "No fue posible encontrar un usuario asociado al correo " + u.nombre;
                return View();
            }
            var contrasennaTemporal = usuarioM.CreatePassword();
            var actualizacion = usuarioM.CambiarContrasenna(respuesta.id, contrasennaTemporal);

            if (!actualizacion)
            {
                ViewBag.msj = "No se pudo actualizar la contraseña. Intente nuevamente.";
                return View();
            }
            string ruta = AppDomain.CurrentDomain.BaseDirectory + "Password.html";
            string contenido = System.IO.File.ReadAllText(ruta);

            contenido = contenido.Replace("@@Nombre", respuesta.nombre);
            contenido = contenido.Replace("@@Contrasenna", contrasennaTemporal);

            usuarioM.EnviarCorreo(respuesta.correo_electronico, "Recuperar Acceso Sistema Incidencias", contenido);

            ViewBag.msj = "Se ha enviado un correo con la nueva contraseña a " + u.correo_electronico;
            return RedirectToAction("Index", "Home");
        }

        public ActionResult registro()
        {
            return View();
        }
        [HttpPost]
        public ActionResult registro(UsuarioEmpleado user)
        {
            var existe = usuarioM.consultaCedulaCorreo(user);

            if (ModelState.IsValid)
            {
                bool registrado = true;
                if (!existe)
                {
                    user.estado_usuario = 1;

                    registrado = usuarioM.RegistrarUsuario(user);

                }

                if (!registrado)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.msj = "Error al registrar el usuario";
                }
            }

            return View(user);
        }

        private UsuarioModel usuarioService = new UsuarioModel();

        public ActionResult ListarUsuarios()
        {
            var usuariosBaseDatos = usuarioService.ListarEmpleados();


            var usuariosEntidades = usuariosBaseDatos.Select(u => new UsuarioEmpleado
            {
                id_usuario = u.id, 
                nombre = u.nombre,
                apellidos = u.apellidos, 
                correo_electronico = u.correo_electronico,
                estado = u.estado_empleado??false, 
                usuario = u.usuario,
                telefono = u.telefono,
                direccion = u.direccion,
                fecha_nacimiento = u.fecha_nacimiento == null ? (DateTime?)null : u.fecha_nacimiento,
                numero_identificacion = u.numero_identificacion

            }).ToList();

            return View(usuariosEntidades);
        }



        public ActionResult Editar(int id)
        {
            try
            {
                var empleado = usuarioM.ObtenerEmpleadoPorId(id);

                if (empleado == null)
                {
                    return HttpNotFound(); 
                }

                ViewBag.Departamentos = usuarioM.ObtenerDepartamentos();
                ViewBag.Puestos = usuarioM.ObtenerListaPuestos();
                ViewBag.Roles = usuarioM.ObtenerListaRoles();

                return View(empleado);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error al cargar el empleado: {ex.Message}");
                return RedirectToAction("Index");
            }
        }

        public ActionResult EditarEmp(int id)
        {
            try
            {
                var empleado = usuarioM.ObtenerEmpleadoPorId(id);

                if (empleado == null)
                {
                    return HttpNotFound();
                }

                ViewBag.Departamentos = usuarioM.ObtenerDepartamentos();
                ViewBag.Puestos = usuarioM.ObtenerListaPuestos();
                ViewBag.Roles = usuarioM.ObtenerListaRoles();

                return View(empleado);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error al cargar el empleado: {ex.Message}");
                return RedirectToAction("Index");
            }
        }

        [HttpPost]

        public ActionResult Editar(Empleado empleado)
        {
            if (ModelState.IsValid)
            {
                var resultado = usuarioM.EditarEmpleado(empleado);
                if (resultado)
                {
                    return RedirectToAction("ListarUsuarios");
                }
                ModelState.AddModelError("", "Error al editar el empleado. Intente nuevamente.");
            }

            ViewBag.Departamentos = usuarioM.ObtenerDepartamentos();
            ViewBag.Puestos = usuarioM.ObtenerListaPuestos();
            ViewBag.Roles = usuarioM.ObtenerListaRoles();

            return View(empleado);
        }

        [HttpPost]

        public ActionResult EditarEmp(Empleado empleado)
        {
            if (ModelState.IsValid)
            {
                var resultado = usuarioM.EditarEmpleado(empleado);
                if (resultado)
                {
                    return RedirectToAction("VerPerfil");
                }
                ModelState.AddModelError("", "Error al editar el empleado. Intente nuevamente.");
            }

            ViewBag.Departamentos = usuarioM.ObtenerDepartamentos();
            ViewBag.Puestos = usuarioM.ObtenerListaPuestos();
            ViewBag.Roles = usuarioM.ObtenerListaRoles();

            return View(empleado);
        }
    }
}
