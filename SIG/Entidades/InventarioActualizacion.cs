using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SIG.Entidades
{
    public class InventarioActualizacion
    {
        public List<SIG.Entidades.Producto> Productos { get; set; }
        public int id { get; set; }
        public string motivo { get; set; }
        public int nuevaCantidad { get; set; }
        public string producto { get; set; }
        public string nombreResponsable { get; set; }
        public DateTime fechaAjuste { get; set; }
    }
}