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
        public Decimal _costo { get; set; }
        public bool _estado { get; set; }
        public DateTime _fInicio { get; set; }
        public DateTime _fFin { get; set; }
        public int _idProyecto { get; set; }
        public List<Material> _materiales { get; set; }
        //public int _idEtapa { get; set; }

    }
}