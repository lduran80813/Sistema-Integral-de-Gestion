using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SIG.Entidades
{
 
        public class PDAUsuarioEmpleado
        {
            public int id_usuario { get; set; }
            public string nombre { get; set; }
            public string apellidos { get; set; }
            public string NombreCompleto
            {
                get
                {
                    return $"{nombre} {apellidos}".Trim();
                }
            }
        }

}