using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SIG.Entidades
{
    public class Ticket
    {
        public int id_ticket { get; set; }
        public int id_usuario { get; set; }
        public int estado { get; set; }
        public int tipo_incidencia { get; set; }
        public int prioridad { get; set; }
        public string titulo { get; set; }
        public string descripcion { get; set; }
        public string comentarios_usuario { get; set; }
        public System.DateTime fecha_registro_usuario { get; set; }
        public System.DateTime fecha_registra_tecnico { get; set; }
        public int id_tecnico { get; set; }
        public string comentario_tecnico { get; set; }
        public System.DateTime fecha_cierre_ticket { get; set; }
    }
}