using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ConstruTecService.Models
{
    /// <summary>
    /// Clase que representa a la tabla ETAPA en la base de datos PostgreSQL
    /// </summary>
    public class Etapa
    {

        public int _id { get; set; }
        public string _nombre { get; set; }
        public string _descripcion { get; set; }

    }
}