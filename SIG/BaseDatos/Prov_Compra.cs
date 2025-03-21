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
    
    public partial class Prov_Compra
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Prov_Compra()
        {
            this.Conta_CxP = new HashSet<Conta_CxP>();
            this.Conta_Transaccion = new HashSet<Conta_Transaccion>();
            this.Prov_CompraDetalle = new HashSet<Prov_CompraDetalle>();
            this.Prov_Pago = new HashSet<Prov_Pago>();
        }
    
        public int id { get; set; }
        public Nullable<int> proveedor_id { get; set; }
        public Nullable<System.DateTime> fecha_compra { get; set; }
        public Nullable<decimal> total_compra { get; set; }
        public string no_factura { get; set; }
        public string notas_adicionales { get; set; }
        public Nullable<int> tipo_compra_id { get; set; }
    
        public virtual Catalogo_Compra Catalogo_Compra { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Conta_CxP> Conta_CxP { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Conta_Transaccion> Conta_Transaccion { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Prov_CompraDetalle> Prov_CompraDetalle { get; set; }
        public virtual Proveedor Proveedor { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Prov_Pago> Prov_Pago { get; set; }
    }
}
