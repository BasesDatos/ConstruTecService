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
        /// Método que permite sumar el costo de cada etapa y obtener el costo total de todo el proyecto
        /// Este costo total es almacenado en la base de datos
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("generateBudget/{id}")]
        [HttpGet]
        public IHttpActionResult generateBudget(int id)
        {
            Decimal totalCost = 0;
            using (NpgsqlConnection connection = DataBase.getConnection())
            {
                NpgsqlCommand command = new NpgsqlCommand("obteneretapasproyecto", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@pidproyecto", NpgsqlDbType.Integer).Value = id;

                try
                {
                    connection.Open();
                    NpgsqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        totalCost += reader.GetDecimal(4);
                    }
                    connection.Close();

                    command = new NpgsqlCommand("generarpresupuesto", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@pidproyecto", NpgsqlDbType.Integer).Value = id;
                    command.Parameters.AddWithValue("@pcostototal", NpgsqlDbType.Numeric).Value = totalCost;
                    connection.Open();
                    reader = command.ExecuteReader();
                    reader.Read();
                    if (reader.GetInt32(0) > 0) { return Json(new Response("Presupuesto generado correctamente")); }
                    else { return Json(new Response("Error al generar el presupuesto")); }
                }
                catch(NpgsqlException ex) { return Json(new Response(Constants.ERROR_DATABASE_CONNECTION)); }
                finally { connection.Close(); }
 
            }
        }





        /// <summary>
        /// Método que permite obtener las etapas asociadas a un proyecto
        /// Se obtiene la información
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("getStagesProjects/{id}")]
        [HttpGet]
        public IHttpActionResult getStagesProject(int id)
        {
            List<Etapa> stages = new List<Etapa>();
            using (NpgsqlConnection connection = DataBase.getConnection())
            {
                NpgsqlCommand command = new NpgsqlCommand("obteneretapasproyecto", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@pidproyecto", NpgsqlDbType.Integer).Value = id;

                try
                {
                    connection.Open();
                    NpgsqlDataReader reader = command.ExecuteReader();
                    int counter = 0;
                    while (reader.Read())
                    {
                        Etapa stage = new Etapa();
                        stage._id = reader.GetInt32(0);
                        stage._nombre = reader.GetString(1);
                        stage._fInicio = reader.GetDateTime(2);
                        stage._fFin = reader.GetDateTime(3);
                        stage._costo = reader.GetDecimal(4);
                        stages.Add(stage);
                        counter++;
                    }
                    if (counter > 0) { return Json(stages); }
                    else { return Json(1); }
                }
                catch (NpgsqlException ex) { return Json(2); }
                finally { connection.Close(); }
            }
        }


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
