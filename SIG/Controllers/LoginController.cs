using SIG.BaseDatos;
using SIG.Entidades;
using SIG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static SIG.Models.UsuarioModel;

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
                // Validar el modelo antes de proceder
                if (!ModelState.IsValid)
                {
                    // Inspeccionar errores del modelo
                    var errores = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();
                    ViewBag.msj = string.Join(", ", errores); // Muestra todos los errores
                    return View(user);
                }

                // Intentar iniciar sesión
                var respuesta = usuarioM.IniciarSesion(user);

                if (respuesta != null)
                {
                    // Almacenar información del usuario en la sesión
                    Session["Usuario"] = respuesta.nombre;
                    Session["IdUsuario"] = respuesta.id;
                    Session["RolUsuario"] = respuesta.rol_id;

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
                // Registrar el error para el seguimiento
                // LogError(ex); // Asegúrate de tener un método para registrar errores.

                ViewBag.msj = "Ocurrió un error al intentar iniciar sesión. Inténtalo de nuevo más tarde.";
                return View(user);
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
            // Verificar si las contraseñas coinciden
            if (u.contrasena != u.contrasenaConfirmar)
            {
                ViewBag.msj = "Las contraseñas no coinciden.";
                return View();  // Retorna la vista actual si las contraseñas no coinciden
            }

            // Obtener el ID del usuario de la sesión
            if (Session["IdUsuario"] == null)
            {
                ViewBag.msj = "Sesión expirada. Por favor, inicie sesión nuevamente.";
                return RedirectToAction("Login", "Login");  // Redirigir al inicio de sesión si no hay ID en la sesión
            }

            int usuarioId = Convert.ToInt32(Session["IdUsuario"]); // Asegúrate de que este ID esté disponible en la sesión

            try
            {
                // Cambiar la contraseña usando el método del usuario
                var resultado = usuarioM.CambiarContrasenna(usuarioId, u.contrasena); // Llama al método que ejecuta el SP

                if (resultado)
                {
                    // La contraseña se cambió exitosamente
                    ViewBag.msj = "La contraseña se ha actualizado exitosamente.";
                    return RedirectToAction("Logout", "Login"); // Redirigir a logout o a donde desees
                }
                else
                {
                    // Mensaje más claro en caso de error
                    ViewBag.msj = $"No se pudo actualizar la contraseña para el usuario con ID {usuarioId}. Asegúrate de que el ID sea correcto.";
                    return View();  // Retorna la vista actual si no se pudo actualizar
                }
            }
            catch (Exception ex)
            {
                ViewBag.msj = $"Ocurrió un error al intentar cambiar la contraseña: {ex.Message}";
                return View(); // Retorna la vista actual en caso de excepción
            }
        }





        [HttpGet]
        public ActionResult RecuperarAcceso()
        {
            return View();
        }

        [HttpPost]
        public ActionResult RecuperarAcceso(UsuarioEmpleado u)
        {
            // Verificar si el objeto usuario está inicializado
            if (u == null || string.IsNullOrWhiteSpace(u.correo_electronico))
            {
                ViewBag.msj = "El objeto usuario no está inicializado o el correo electrónico no es válido.";
                return View(); // o redirigir según sea necesario
            }

            // Validar si el correo existe
            var respuesta = usuarioM.ValidarCorreo(u.usuario);

            if (respuesta == null)
            {
                ViewBag.msj = "No fue posible encontrar un usuario asociado al correo " + u.usuario;
                return View();
            }

            // Generar una nueva contraseña temporal
            var contrasennaTemporal = usuarioM.CreatePassword();

            // Actualizar la contraseña del usuario
            var actualizacion = usuarioM.CambiarContrasenna(respuesta.id, contrasennaTemporal);

            if (!actualizacion)
            {
                ViewBag.msj = "No se pudo actualizar la contraseña. Intente nuevamente.";
                return View();
            }

            // Leer el contenido del email
            string ruta = AppDomain.CurrentDomain.BaseDirectory + "Password.html";
            string contenido = System.IO.File.ReadAllText(ruta);

            // Reemplazar los marcadores en el contenido del correo
            contenido = contenido.Replace("@@Nombre", respuesta.usuario);
            contenido = contenido.Replace("@@Contrasenna", contrasennaTemporal);

            // Enviar el correo con la nueva contraseña
            usuarioM.EnviarCorreo(respuesta.usuario, "Recuperar Acceso Sistema Incidencias", contenido);

            // Redirigir a la página de login con un mensaje de éxito
            ViewBag.msj = "Se ha enviado un correo con la nueva contraseña a " + u.usuario;
            return RedirectToAction("Index", "Home");
        }


    }
}