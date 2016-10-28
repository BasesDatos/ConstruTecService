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
        /// Método que permite obtener los materiales de una étapa
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("getMaterialsStage/{id}")]
        [HttpGet]
        public IHttpActionResult getMaterialsStage(int id)
        {
            List<Material> materials = new List<Material>();
            using (NpgsqlConnection connection = DataBase.getConnection())
            {
                NpgsqlCommand command = new NpgsqlCommand("obtenermaterialesetapa", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@pidrelacion", connection).Value = id;

                try
                {
                    connection.Open();
                    NpgsqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Material material = new Material();
                        material._id = reader.GetInt64(0);
                        material._nombre = reader.GetString(1);
                        material._precio = reader.GetDecimal(2);
                        material._cantidadDisponible = reader.GetInt32(3);
                        materials.Add(material);
                    }
                    return Json(materials);
                }
                catch (NpgsqlException ex) { return Json(2); }
                finally { connection.Close(); }
            }
        }



        /// <summary>
        /// Método que permite marcar una etapa como finalizada
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("endStage")]
        [HttpGet]
        public IHttpActionResult endStage(int id)
        {
            using (NpgsqlConnection connection = DataBase.getConnection())
            {
                NpgsqlCommand command = new NpgsqlCommand("marcaretapa", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@pidrelacion", NpgsqlDbType.Integer).Value = id;

                try
                {
                    connection.Open();
                    NpgsqlDataReader reader = command.ExecuteReader();
                    reader.Read();
                    var result = reader.GetString(0);

                    return Json(new Response(result));
                }
                catch (NpgsqlException ex) { return Json(new Response(Constants.ERROR_DATABASE_CONNECTION)); }
                finally { connection.Close(); }
            }
        }




        /// <summary>
        /// Método que permite agregar los materiales que se utilizaran en un etapa
        /// Con esta información, cuando el usuario lo desee se hara el pedido correspondiente
        /// en el web service de EPATEC
        /// </summary>
        /// <param name="pEtapa"></param>
        /// <returns></returns>
        [Route("addMaterials")]
        [HttpPost]
        public IHttpActionResult addMaterials(Etapa pEtapa)
        {
            using (NpgsqlConnection connection = DataBase.getConnection())
            {
                try
                {
                    int result = 0;
                    List<Material> materiales = pEtapa._materiales;
                    for(int i=0; i<materiales.Count(); i++)
                    {
                        NpgsqlCommand command = new NpgsqlCommand("agregarmaterial", connection);
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@pidmaterial", NpgsqlDbType.Integer).Value = materiales.ElementAt(i)._id;
                        command.Parameters.AddWithValue("@pnombrematerial", NpgsqlDbType.Text).Value = materiales.ElementAt(i)._nombre;
                        command.Parameters.AddWithValue("@pidrelacionetapa", NpgsqlDbType.Integer).Value = pEtapa._id;
                        command.Parameters.AddWithValue("@pprecio", NpgsqlDbType.Integer).Value = materiales.ElementAt(i)._precio;
                        command.Parameters.AddWithValue("@pcantidad", NpgsqlDbType.Integer).Value = materiales.ElementAt(i)._cantidadDisponible;
                        connection.Open();
                        NpgsqlDataReader reader = command.ExecuteReader();
                        reader.Read();
                        if(reader.GetInt32(0) > 0) { result++; }
                        connection.Close();
                    }

                    if (result == materiales.Count) { return Json(new Response("Materiales agregados con éxito")); }
                    else return Json(new Response("Algunos materiales no fueron agregados debido a  un error"));
                }
                catch(NpgsqlException ex) { return Json(new Response(ex.Message)); }
                //finally { connection.Close(); }
            }
        }




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
                    pEtapa._id, pEtapa._fInicio.ToString("dd/MM/yyyy"), pEtapa._fFin.ToString("dd/MM/yyyy"));

                NpgsqlCommand command = new NpgsqlCommand(parameters.ToString(), connection);
                command.CommandType = CommandType.Text;

                try
                {
                    connection.Open();
                    NpgsqlDataReader reader = command.ExecuteReader();
                    reader.Read();

                    int result = reader.GetInt32(0);
                    switch (result)
                    {
                        case -1: return Json(new Response("La etapa a asociar no existe"));
                        case -2: return Json(new Response("El proyecto asociado no existe"));
                        default: return Json(new Etapa { _id = result });
                    }

                }
                catch (NpgsqlException ex) { return Json(new Response(Constants.ERROR_DATABASE_CONNECTION)); }
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
