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
    
    public partial class Venta_Factura
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Venta_Factura()
        {
            this.Venta_FacturaDetalle = new HashSet<Venta_FacturaDetalle>();
        }
    
        public int id { get; set; }
        public Nullable<int> cliente_id { get; set; }
        public Nullable<System.DateTime> fecha_venta { get; set; }
        public Nullable<int> metodo_pago_id { get; set; }
        public Nullable<decimal> total_transaccion { get; set; }
        public string no_factura { get; set; }
        public string direccion { get; set; }
        public string estado_factura { get; set; }
        public string notas_adicionales { get; set; }
    
        public virtual Venta_Cliente Venta_Cliente { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Venta_FacturaDetalle> Venta_FacturaDetalle { get; set; }
        public virtual Venta_MetodoPago Venta_MetodoPago { get; set; }
    }
}
