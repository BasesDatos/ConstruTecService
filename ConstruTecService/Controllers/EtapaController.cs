using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Data;
using Npgsql;
using NpgsqlTypes;
using System.Web.Http;
using ConstruTecService.Models;

namespace ConstruTecService.Controllers
{
    [RoutePrefix("stages")]
    public class EtapaController : ApiController
    {

        /// <summary>
        /// Método que permite registar una nueva etapa en la base de datos, la cual se agregara
        /// a la tabla ETAPA, junto con las etapas ofrecidad por default
        /// </summary>
        /// <param name="pEtapa"></param>
        /// <returns></returns>
        [Route("register")]
        [HttpPost]
        public IHttpActionResult register(Etapa pEtapa)
        {
            using (NpgsqlConnection connection = DataBase.getConnection())
            {
                NpgsqlCommand command = new NpgsqlCommand("registraretapa", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@pnombre", NpgsqlDbType.Text).Value = pEtapa._nombre;
                command.Parameters.AddWithValue("@pdescripcion", NpgsqlDbType.Text).Value = pEtapa._descripcion;

                try
                {
                    connection.Open();
                    NpgsqlDataReader reader = command.ExecuteReader();
                    reader.Read();

                    return Json(new Response(reader.GetString(0)));
                }
                catch(NpgsqlException ex) { return Json(new Response(Constants.ERROR_DATABASE_CONNECTION)); }
                finally { connection.Close(); }
            }
        }



    }
}
