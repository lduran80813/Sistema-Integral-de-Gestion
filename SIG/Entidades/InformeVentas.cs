using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SIG.Entidades
{
    public class InformeVentas
    {
        public System.DateTime inicioCorte { get; set; }
        public System.DateTime finCorte { get; set; }
        public List<SIG.BaseDatos.Informe_Ventas_Result> datosInforme { get; set; }
        public int cantidadVentas { get; set; }
        public List<Producto> productosMasVendidos { get; set; }
        public decimal ingresosTotales { get; set; }

        public List<SIG.BaseDatos.top5Clientes_Result> principalesClientes { get; set; }

        public List<SIG.BaseDatos.distribMetodoPago_Result> distribucionMediosPago { get; set; }
    }
}