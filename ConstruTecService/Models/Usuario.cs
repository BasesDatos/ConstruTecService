using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ConstruTecService.Models
{
    /// <summary>
    /// Clase que representa a la tabla USUARIO en la base de datos PostgreSQL
    /// </summary>
    public class Usuario
    {

        public string _usuario { get; set; }
        public string _nombre { get; set; }
        public string _pApellido { get; set; }
        public string _sApellido { get; set; }
        public Int64 _cedula { get; set; }
        public string _contrasena { get; set; }
        public string _codigo { get; set; } = "";
        public string _telefono { get; set; }
        public int _rol { get; set; }
    }
}