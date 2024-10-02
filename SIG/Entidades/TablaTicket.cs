using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SIG.Entidades
{
    public class TablaTicket 
    { 
        public int id_ticket { get; set; }
        public string usuario { get; set; }
        public string estado { get; set; }
        public string tipo_incidencia { get; set; }
        public string prioridad { get; set; }
        public System.DateTime fecha_registro_usuario { get; set; }
        public System.DateTime fecha_registra_tecnico { get; set; }
        public string tecnico { get; set; }
        public System.DateTime fecha_cierre_ticket { get; set; }
    }
}