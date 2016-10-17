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

        /* ---------------------------------------- ERRORS ---------------------------------------- */
        public static string ERROR_DATABASE_CONNECTION { get { return "Error de conexión con la base de datos"; } }
        public static string ERROR_SQLSERVER_CONNECTION { get { return "Error en la conexión con el web service de EPATEC"; } }
        public static string ERROR_ROL_OUT_OF_BOUNDS { get { return "Error: No existe este rol en el sisetma"; } }
        public static String ERROR_USER_PASS { get { return "Usuario o contraseña incorrectos"; } }


        /* ---------------------------------------- ROLES ---------------------------------------- */
        public static int GENERAL_USER { get { return 1; } }
        public static int ENGINEER { get { return 2; } }
        public static int ADMIN { get { return 3; } }

        public static string URL_EPATEC_SERVICE { get { return "http://localhost:8080/products/all"; } }
    }
}
 