//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SIG.BaseDatos
{
    using System;
    
    public partial class ConsultarCarrito_Result
    {
        public int IdCarrito { get; set; }
        public int IdUsuario { get; set; }
        public int id { get; set; }
        public string descripcion { get; set; }
        public decimal Precio { get; set; }
        public Nullable<decimal> SubTotal { get; set; }
        public Nullable<decimal> Impuesto { get; set; }
        public Nullable<decimal> Total { get; set; }
        public int Cantidad { get; set; }
        public System.DateTime Fecha { get; set; }
    }
}
