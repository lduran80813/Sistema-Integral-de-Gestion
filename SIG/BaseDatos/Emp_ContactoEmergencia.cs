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
    
    public partial class Emp_ContactoEmergencia
    {
        public int id { get; set; }
        public Nullable<int> empleado_id { get; set; }
        public string nombre_contacto { get; set; }
        public string telefono_contacto { get; set; }
        public string relacion { get; set; }
    
        public virtual Empleado Empleado { get; set; }
    }
}
