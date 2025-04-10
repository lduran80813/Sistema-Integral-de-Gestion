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
    
    public partial class Pedido
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Pedido()
        {
            this.HistorialModificacion = new HashSet<HistorialModificacion>();
            this.HistorialPedido = new HashSet<HistorialPedido>();
        }
    
        public int id { get; set; }
        public System.DateTime fecha_pedido { get; set; }
        public int proveedor_id { get; set; }
        public decimal total_pedido { get; set; }
        public int EstadoCompraId { get; set; }
        public int TipoCompraId { get; set; }
    
        public virtual EstadoCompra EstadoCompra { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HistorialModificacion> HistorialModificacion { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HistorialPedido> HistorialPedido { get; set; }
        public virtual Proveedor Proveedor { get; set; }
    }
}
