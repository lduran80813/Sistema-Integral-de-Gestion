using SIG.BaseDatos;
using System;
using System.Collections.Generic;

namespace SIG.Entidades
{
    public class PlanDeAccion
    {
        public int Id { get; set; }
        public string NombrePlan { get; set; }
        public string DescripcionPlan { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFinalizacion { get; set; }
        public string Estado { get; set; }

        // Relación con Tareas
        public ICollection<Tarea> Tareas { get; set; } = new List<Tarea>();

        // Relación con Categoría
        public int? CategoriaId { get; set; }
        public CategoriaPlan Categoria { get; set; }

        public int? ResponsableId { get; set; }  // ResponsableId es una clave foránea a la tabla Empleado
        public Empleado Responsable { get; set; }  // Relación de navegación con la entidad Empleado
    }
}
