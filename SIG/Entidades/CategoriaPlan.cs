using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SIG.Entidades
{
    public class CategoriaPlan
    {
        public int Id { get; set; }
        public string NombreCategoria { get; set; }
        public string Descripcion { get; set; }
        public bool Estado { get; set; }  // Activo / Inactivo
    }
} 