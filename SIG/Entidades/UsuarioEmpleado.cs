﻿using System;
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


        public string nombre { get; set; }


        public string apellidos { get; set; }


        public string numero_identificacion { get; set; }


        public DateTime? fecha_nacimiento { get; set; }


        public string direccion { get; set; }


        public string telefono { get; set; }


        public string correo_electronico { get; set; }

        public byte[] contrasenaBytes { get; set; }
        public int? departamento_id { get; set; }  // Permitir valores nulos
        public int? puesto_id { get; set; }        // Permitir valores nulos
        public int? rol_id { get; set; }

        public int estado_usuario { get; set; }
        public bool estado { get; set; }

        public string nombre_departamento { get; set; }
        public string nombre_puesto { get; set; }
        public string nombre_rol { get; set; }

    }


}