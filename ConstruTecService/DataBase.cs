using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Npgsql;

namespace ConstruTecService
{
    public class DataBase
    {

        //String de conexión a la base de datos
        private static string stringConnection = "Server=127.0.0.1; User Id=postgres; Password=gremory1212951995; Database=CONSTRU-TEC";

        /// <summary>
        /// Método utilizado por los controladores para obtener una conexión a la base de datos
        /// </summary>
        /// <returns>Conexión con PostgreSQL</returns>
        public static NpgsqlConnection getConnection()
        {
            NpgsqlConnection connection = new NpgsqlConnection(stringConnection);

            return connection;
        }
    }
}