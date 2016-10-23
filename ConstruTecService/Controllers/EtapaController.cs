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
using System.Text;

namespace ConstruTecService.Controllers
{
    [RoutePrefix("stages")]
    public class EtapaController : ApiController
    {

        /// <summary>
        /// Método que permite obtener el listado con todas las etapas ofrecidas por el sistema, junto con las etapas
        /// que hallan sido agregadas luego por ingenieros o administradores
        /// </summary>
        /// <returns></returns>
        [Route("all")]
        [HttpGet]
        public IHttpActionResult all()
        {
            List<Etapa> stages = new List<Etapa>();
            using (NpgsqlConnection connection = DataBase.getConnection())
            {
                NpgsqlCommand command = new NpgsqlCommand("listaretapas", connection);
                command.CommandType = CommandType.StoredProcedure;

                try
                {
                    connection.Open();
                    NpgsqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Etapa stage = new Etapa();
                        stage._id = reader.GetInt32(0);
                        stage._nombre = reader.GetString(1);
                        stage._descripcion = reader.GetString(2);
                        stages.Add(stage);
                    }

                    return Json(stages);
                }
                catch(NpgsqlException ex) { return Json(new Response(Constants.ERROR_DATABASE_CONNECTION)); }
                finally { connection.Close(); }
            }
        }


        /// <summary>
        /// Método que permite asociar una etapa a un proyecto, junto con información como la fecha de inicio
        /// y finalización de la misma
        /// </summary>
        /// <param name="pEtapa"></param>
        /// <returns></returns>
        [Route("associateStage")]
        [HttpPost]
        public IHttpActionResult associateStage(Etapa pEtapa)
        {
            using (NpgsqlConnection connection = DataBase.getConnection())
            {

                StringBuilder parameters = new StringBuilder();
                parameters.AppendFormat("select * from asociarEtapa({0}, {1},'{2}','{3}')", pEtapa._idProyecto, 
                    pEtapa._idEtapa, pEtapa._fInicio.ToString("dd/MM/yyyy"), pEtapa._fFin.ToString("dd/MM/yyyy"));

                NpgsqlCommand command = new NpgsqlCommand(parameters.ToString(), connection);
                command.CommandType = CommandType.Text;

                try
                {
                    connection.Open();
                    NpgsqlDataReader reader = command.ExecuteReader();
                    reader.Read();

                    return Json(new Response(reader.GetString(0)));
                }
                catch (NpgsqlException ex) { return Json(new Response(ex.Message)); }
                finally { connection.Close(); }
            }
        }




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
