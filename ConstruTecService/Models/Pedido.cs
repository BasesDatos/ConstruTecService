using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ConstruTecService.Models
{
    public class Pedido
    {

        public TimeSpan _horaPedido { get; set; }
        public DateTime _fechaPedido { get; set; }
        public int _idRelacionEtapa { get; set; }
    }
}