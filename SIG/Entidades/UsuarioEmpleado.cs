using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SIG.Entidades
{
    public class UsuarioEmpleado
    {
        public int id_usuario { get; set; }
        public string usuario { get; set; }
        public string contrasena { get; set; }
        public int rol { get; set; }
        public int empleado_id { get; set; }
        public string nombre_completo { get; set; }
        public string contrasenaConfirmar { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        public string nombre { get; set; }

        [Required(ErrorMessage = "Los apellidos son obligatorios.")]
        public string apellidos { get; set; }

        [Required(ErrorMessage = "El número de identificación es obligatorio.")]
        public string numero_identificacion { get; set; }

        [Required(ErrorMessage = "La fecha de nacimiento es obligatoria.")]
        public DateTime fecha_nacimiento { get; set; }

        [Required(ErrorMessage = "La dirección es obligatoria.")]
        public string direccion { get; set; }

        [Required(ErrorMessage = "El teléfono es obligatorio.")]
        public string telefono { get; set; }

        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "El correo electrónico no es válido.")]
        public string correo_electronico { get; set; }

        [Required(ErrorMessage = "El departamento es obligatorio.")]
        public int departamento_id { get; set; }

        [Required(ErrorMessage = "El puesto es obligatorio.")]
        public int puesto_id { get; set; }
        public string email { get; set; }

    }
}