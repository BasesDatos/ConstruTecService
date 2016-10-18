using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data;
using ConstruTecService.Models;
using Npgsql;
using NpgsqlTypes;

namespace ConstruTecService.Controllers
{
    [RoutePrefix("projects")]
    public class ProyectoController : ApiController
    {

        /// <summary>
        /// Método que permite registrar un nuevo proyecto en la base de datos
        /// </summary>
        /// <param name="pProject"></param>
        /// <returns>Id con el que se registro el proyecto</returns>
        [Route("register")]
        [HttpPost]
        public IHttpActionResult register(Proyecto pProject)
        {
            using (NpgsqlConnection connection = DataBase.getConnection())
            {
                NpgsqlCommand command = new NpgsqlCommand("registrarproyecto", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@pnombre", NpgsqlDbType.Text).Value = pProject._nombre;
                command.Parameters.AddWithValue("@pprovincia", NpgsqlDbType.Text).Value = pProject._provincia;
                command.Parameters.AddWithValue("@pcanton", NpgsqlDbType.Text).Value = pProject._canton;
                command.Parameters.AddWithValue("@pdistrito", NpgsqlDbType.Text).Value = pProject._distrito;
                command.Parameters.AddWithValue("@pcliente", NpgsqlDbType.Text).Value = pProject._cliente;
                command.Parameters.AddWithValue("@pingeniero", NpgsqlDbType.Text).Value = pProject._ingeniero;

                try
                {
                    connection.Open();
                    NpgsqlDataReader reader = command.ExecuteReader();
                    reader.Read();

                    return Json(new Response(reader.GetString(0)));
                }
                catch (NpgsqlException ex) { return Json(new Response(Constants.ERROR_DATABASE_CONNECTION)); }
                finally { connection.Close(); }
            }
        }



    }
}
