using System;
using System.Collections.Generic;
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

        public string email { get; set; }
    }
}