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

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(UsuarioEmpleado user)
        {
            try
            {
                var respuesta = usuarioM.IniciarSesion(user);

                if (respuesta != null)
                {
                    Session["Usuario"] = respuesta.usuario;
                    Session["IdUsuario"] = respuesta.id;
                    Session["RolUsuario"] = respuesta.rol_id;

                    return RedirectToAction("Index", "Home"); 
                }
                else
                {

                    ViewBag.msj = "Credenciales incorrectas";
                }
            }
            catch (UsuarioNoEncontradoException ex)
            {
                ViewBag.msj = ex.Message;
            }

           
            return View();
        }


        [HttpGet]
        public ActionResult CambiarContrasenna()
        {
            return View();
        }

        [FiltroSeguridad]
        [HttpGet]
        public ActionResult Logout()
        {
            Session.Abandon();

            return RedirectToAction("Login", "Login");
        }


        [HttpPost]
        public ActionResult CambiarContrasenna(UsuarioEmpleado u)
        {
            if (u.contrasena != u.contrasenaConfirmar)
            {
                ViewBag.msj = "Las contraseñas no coinciden";
                return View();
            }

            string usuario = Session["Usuario"].ToString();

            var resultado = usuarioM.CambiarContrasenna(usuario, u.contrasena);

            if (resultado)
            {

                return RedirectToAction("Logout", "Login");
            }
            else
            {
                ViewBag.msj = "No ha sido posible actualizar su contraseña";
                return View();
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
            if (u == null || string.IsNullOrWhiteSpace(u.usuario))
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
            var actualizacion = usuarioM.CambiarContrasenna(respuesta.usuario, contrasennaTemporal);

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