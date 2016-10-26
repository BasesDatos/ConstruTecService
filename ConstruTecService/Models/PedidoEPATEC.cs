using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace ConstruTecService.Models
{
    public class PedidoEPATEC
    {

        public Int64 _idPedido { get; set; }

        [DataMember]
        public string _idCliente { get; set; } = "Unregistered";

        [DataMember]
        public TimeSpan _horaPedido { get; set; }

        [DataMember]
        public DateTime _fechaPedido { get; set; }

        [DataMember]
        public List<Material> _productos { get; set; }

        [DataMember]
        public Decimal _total { get; set; }
    }
}