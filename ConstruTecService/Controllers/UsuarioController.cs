using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Npgsql;
using ConstruTecService.Models;

namespace ConstruTecService.Controllers
{
    [RoutePrefix("users")]
    public class UsuarioController : ApiController
    {

        /// <summary>
        /// Método que permite obtener la información de usuarios generales, ingenieros y administradores
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("all/{id}")]
        [HttpGet]
        public IHttpActionResult all(int id)
        {
            List<Usuario> _users = null;
            switch (id)
            {
                case 1:
                    _users = allGeneral(Constants.GENERAL_USER); //Obtener usuarios generales
                    break;
                case 3:
                    _users = allGeneral(Constants.ADMIN); //Obtener administradores
                    break;
                case 2:
                    _users = allEngineers();
                    break;
                default:
                    return Json(new Response(Constants.ERROR_ROL_OUT_OF_BOUNDS));
            }

            if (_users != null) return Json(_users);
            else { return Json(new Response(Constants.ERROR_DATABASE_CONNECTION)); }
        }




        /// <summary>
        /// Método que se comunica con la base de datos para obtener la información de todos los usuarios generales,
        /// utilizando el procedimiento almacenado "listarUsuariosGenerales"
        /// </summary>
        /// <returns></returns>
        private List<Usuario> allGeneral(int pRol)
        {

            List<Usuario> _users = new List<Usuario>();
            string procedure = "";
            if (pRol == 1) procedure = "listarUsuariosGenerales";
            else procedure = "listarAdministradores";

            using (NpgsqlConnection connection = DataBase.getConnection())
            {

                NpgsqlCommand command = new NpgsqlCommand(procedure, connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;

                try
                {
                    connection.Open();
                    NpgsqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        Usuario user = new Usuario();
                        user._nombre = reader.GetString(0);
                        user._pApellido = reader.GetString(1);
                        user._sApellido = reader.GetString(2);
                        user._usuario = reader.GetString(3);
                        user._cedula = reader.GetInt64(4);
                        user._contrasena = reader.GetString(5);
                        user._telefono = reader.GetString(6);
                        _users.Add(user);
                    }

                    return _users;
                }
                catch (NpgsqlException ex) { return null; }
                finally { connection.Close(); }
            }
        }



        /// <summary>
        /// Método que obtiene la información de todos los ingenieros/arquitectos, 
        /// utilizando el procedimientos almacenado "listarIngenieros"
        /// </summary>
        /// <returns></returns>
        private List<Usuario> allEngineers()
        {
            List<Usuario> _engineers = new List<Usuario>();

            using (NpgsqlConnection connection = DataBase.getConnection())
            {

                NpgsqlCommand command = new NpgsqlCommand("listarIngenieros", connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;

                try
                {
                    connection.Open();
                    NpgsqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        Usuario user = new Usuario();
                        user._nombre = reader.GetString(0);
                        user._pApellido = reader.GetString(1);
                        user._sApellido = reader.GetString(2);
                        user._usuario = reader.GetString(3);
                        user._cedula = reader.GetInt64(4);
                        user._contrasena = reader.GetString(5);
                        user._codigo = reader.GetString(6);
                        user._telefono = reader.GetString(7);
                        _engineers.Add(user);
                    }

                    return _engineers;
                }
                catch (NpgsqlException ex) { return null; }
                finally { connection.Close(); }
            }
        }









    }
}
