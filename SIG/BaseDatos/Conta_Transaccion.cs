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
    
    public partial class Conta_Transaccion
    {
        public int id { get; set; }
        public Nullable<int> compra_id { get; set; }
        public Nullable<int> venta_id { get; set; }
        public Nullable<System.DateTime> fecha { get; set; }
        public Nullable<decimal> monto { get; set; }
        public string descripcion { get; set; }
        public int cuenta_contable { get; set; }
        public string entidad_financiera { get; set; }
        public Nullable<decimal> transaccion_referencia { get; set; }
        public int usuario_id { get; set; }
    
        public virtual Conta_CuentasContables Conta_CuentasContables { get; set; }
        public virtual Prov_Compra Prov_Compra { get; set; }
        public virtual EntidadesFinancieras EntidadesFinancieras { get; set; }
        public virtual Empleado Empleado { get; set; }
        public virtual Venta_Factura Venta_Factura { get; set; }
    }
}
