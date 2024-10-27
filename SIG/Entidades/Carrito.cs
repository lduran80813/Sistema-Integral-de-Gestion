using SIG.BaseDatos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SIG.Entidades
{
    public class Carrito
    {        public int IdProducto { get; set; }
        public List<ConsultarCarrito_Result> DatosCarrito { get; set; }
    }
}