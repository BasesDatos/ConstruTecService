using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ConstruTecService.Models
{
    public class Detalle
    {

        public int _id { get; set; }
        public Decimal _costo { get; set; }
        public bool _estado { get; set; }
        public DateTime _fInicio { get; set; }
        public DateTime _fFin { get; set; }
        public int _idProyecto { get; set; }
    }
}