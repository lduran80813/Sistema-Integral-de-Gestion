using SIG.BaseDatos;
using SIG.Entidades;
using SIG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
                return View();
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
                ViewBag.msj = "Las contraseñas no coinciden";
                return View();
            }

            // Obtener el usuario de la sesión
            string usuario = Session["Usuario"].ToString();

            var resultado = usuarioM.CambiarContrasenna(usuario, u.contrasena);

            if (resultado)
            {
                // La contraseña se cambió exitosamente
                return RedirectToAction("Logout", "Login");
            }
            else
            {
                ViewBag.msj = "No ha sido posible actualizar su contraseña";
                return View();
            }
        }




    }
}