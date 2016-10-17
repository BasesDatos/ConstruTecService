using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ConstruTecService.Models
{

    /// <summary>
    /// Clase que representa la tabla material en la base de datos PostgreSQL
    /// </summary>
    public class Material
    {

        public Int64 _id { get; set; }
        public string _nombre { get; set; }
        public string _descripcion { get; set; }
        public int _cantidadDisponible { get; set; }
        public Decimal _precio { get; set; }

    }
}