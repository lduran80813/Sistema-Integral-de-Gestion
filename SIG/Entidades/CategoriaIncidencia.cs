using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SIG.Entidades
{
    public class CategoriaIncidencia
    {
        public int tipo_incidencia { get; set; }
        public string descripcion { get; set; }
        public bool estado { get; set; }
    }
}