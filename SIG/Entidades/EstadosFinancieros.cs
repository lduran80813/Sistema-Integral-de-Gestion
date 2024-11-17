using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SIG.Entidades
{
    public class EstadosFinancieros
    {
        public DateTime mesCorte {  get; set; }
        public List<SIG.BaseDatos.GenerarBalanceGeneral_Result> BalanceGeneral { get; set; }
        public List<SIG.BaseDatos.GenerarEstadoResultados_Result> EstadoResultados { get; set; }

    }
}