using SIG.BaseDatos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SIG.Entidades
{
    public class EmpCapacitacionParticipantes
    {
        public int id { get; set; }
        public int capacitacion_id { get; set; }
        public int participante_id { get; set; }
        public Empleado participante { get; set; }  // Relación de navegación con la entidad Empleado
        public int calificacion { get; set; }
        public string retroalimentacion { get; set; }

        public EmpCapacitacion Capacitacion { get; set; }
        public PDAEmpleado Participante { get; set; }
    }
}