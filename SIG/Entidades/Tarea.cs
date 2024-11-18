using SIG.BaseDatos;
using SIG.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SIG.Entidades
{
    public class Tarea
    {
        public int Id { get; set; }
        public int PlanId { get; set; }
        public string DescripcionTarea { get; set; }
        public int ResponsableId { get; set; }
        public int EstadoTareaId { get; set; }  // Relación con PDAEstado

        // Relaciones
        public PlanDeAccion Plan { get; set; }
        public PDAEmpleado Responsable { get; set; }
        public PDAEstado EstadoTarea { get; set; }  // Referencia a PDAEstado
    }
}