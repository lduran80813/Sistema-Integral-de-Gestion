//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SIG.BaseDatos
{
    using System;
    using System.Collections.Generic;
    
    public partial class Empleado
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Empleado()
        {
            this.Emp_ContactoEmergencia = new HashSet<Emp_ContactoEmergencia>();
            this.Emp_RemuneracionDeduccion = new HashSet<Emp_RemuneracionDeduccion>();
            this.Emp_Vacaciones = new HashSet<Emp_Vacaciones>();
            this.PDA_Tarea = new HashSet<PDA_Tarea>();
            this.Usuario = new HashSet<Usuario>();
        }
    
        public int id { get; set; }
        public string nombre { get; set; }
        public string apellidos { get; set; }
        public string numero_identificacion { get; set; }
        public Nullable<System.DateTime> fecha_nacimiento { get; set; }
        public string direccion { get; set; }
        public string telefono { get; set; }
        public string correo_electronico { get; set; }
        public Nullable<int> departamento_id { get; set; }
        public Nullable<int> puesto_id { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Emp_ContactoEmergencia> Emp_ContactoEmergencia { get; set; }
        public virtual Emp_Departamento Emp_Departamento { get; set; }
        public virtual Emp_Puesto Emp_Puesto { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Emp_RemuneracionDeduccion> Emp_RemuneracionDeduccion { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Emp_Vacaciones> Emp_Vacaciones { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PDA_Tarea> PDA_Tarea { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Usuario> Usuario { get; set; }
    }
}