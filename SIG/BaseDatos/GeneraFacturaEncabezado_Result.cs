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
    
    public partial class GeneraFacturaEncabezado_Result
    {
        public int Factura_id { get; set; }
        public Nullable<int> cliente_id { get; set; }
        public string nombreCliente { get; set; }
        public string direccion { get; set; }
        public string telefono { get; set; }
        public string correo_electronico { get; set; }
        public Nullable<System.DateTime> fecha { get; set; }
        public decimal monto { get; set; }
        public decimal impuesto { get; set; }
        public Nullable<decimal> total_transaccion { get; set; }
        public string MetodoPago { get; set; }
        public string EstadoFactura { get; set; }
        public string notas_adicionales { get; set; }
        public string TipoVenta { get; set; }
    }
}