using SIG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SIG.Entidades
{
    public class VentaPedido
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }
        public DateTime FechaPedido { get; set; }
        public string NumeroPedido { get; set; }

        internal static object Select(Func<object, EntregasModel> value)
        {
            throw new NotImplementedException();
        }
    }
}