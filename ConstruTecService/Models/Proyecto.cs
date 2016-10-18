using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ConstruTecService.Models
{

    /// <summary>
    /// Clase representa a la table PROYECTO en la base de datos PostgreSQL
    /// </summary>
    public class Proyecto
    {

        public int _id { get; set; }
        public string _nombre { get; set; }
        public bool _estado { get; set; }
        public Decimal _costo { get; set; }
        public string _provincia { get; set; }
        public string _canton { get; set; }
        public string _distrito { get; set; }
        public string _cliente { get; set; }
        public string _ingeniero { get; set; }

    }
}