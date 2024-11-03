using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SIG.Entidades
{
    public class Proveedor
    {
        public int id { get; set; }
        public string nombre { get; set; }
        public string direccion { get; set; }
        public int estado { get; set; }
        public string nombre_contacto { get; set; }
        public string telefono { get; set; }
        public string correo_electronico { get; set; }
        public string descripcion_adicional { get; set; }
    }
}