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
    
    public partial class Prov_Producto
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Prov_Producto()
        {
            this.Prov_CompraDetalle = new HashSet<Prov_CompraDetalle>();
        }
    
        public int id { get; set; }
        public string nombre { get; set; }
        public string descripcion { get; set; }
        public Nullable<decimal> precio { get; set; }
        public Nullable<System.DateTime> fecha { get; set; }
        public Nullable<int> proveedor_id { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Prov_CompraDetalle> Prov_CompraDetalle { get; set; }
        public virtual Proveedor Proveedor { get; set; }
    }
}
