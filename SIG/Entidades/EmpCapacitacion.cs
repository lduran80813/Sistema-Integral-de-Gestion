using SIG.BaseDatos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SIG.Entidades
{
    public class EmpCapacitacion
    {
        public int id { get; set; }
        public string nombre_capacitacion { get; set; }
        public Nullable<System.DateTime> fecha_inicio { get; set; }
        public Nullable<System.DateTime> fecha_finalizacion { get; set; }
        public string descripcion_capacitacion { get; set; }
        public Nullable<int> ResponsableId { get; set; }
        public Empleado Responsable { get; set; }  // Relación de navegación con la entidad Empleado

        public ICollection<EmpCapacitacionParticipantes> Participantes { get; set; } = new List<EmpCapacitacionParticipantes>();
    }
}