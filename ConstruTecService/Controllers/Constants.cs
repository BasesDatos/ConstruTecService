using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ConstruTecService.Controllers
{

    /// <summary>
    /// Clase que almacena constantes utilizadas por los controladores
    /// </summary>
    public static class Constants
    {

        public static string ERROR_DATABASE_CONNECTION { get { return "Error de conexión con la base de datos"; } }
        public static string ERROR_ROL_OUT_OF_BOUNDS { get { return "Error: No existe este rol en el sisetma"; } }

        public static int GENERAL_USER { get { return 1; } }
        public static int ENGINEER { get { return 2; } }
        public static int ADMIN { get { return 3; } }
    }
}