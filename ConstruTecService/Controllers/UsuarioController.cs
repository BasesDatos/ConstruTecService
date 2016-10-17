using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data;
using Npgsql;
using NpgsqlTypes;
using ConstruTecService.Models;

namespace ConstruTecService.Controllers
{
    [RoutePrefix("users")]
    public class UsuarioController : ApiController
    {

        /// <summary>
        /// Método que permite el registro de los usuarios, tanto usuario generales y administradores,
        /// como ingenieros
        /// </summary>
        /// <param name="pUser"></param>
        /// <returns></returns>
        [Route("register")]
        [HttpPost]
        public IHttpActionResult register(Usuario pUser)
        {
            using (NpgsqlConnection connection = DataBase.getConnection())
            {
                NpgsqlCommand command = new NpgsqlCommand("registrarusuario", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@pusuario", NpgsqlDbType.Text).Value = pUser._usuario;
                command.Parameters.AddWithValue("@pnombre", NpgsqlDbType.Text).Value = pUser._nombre;
                command.Parameters.AddWithValue("@ppapellido", NpgsqlDbType.Text).Value = pUser._pApellido;
                command.Parameters.AddWithValue("@psapellido", NpgsqlDbType.Text).Value = pUser._sApellido;
                command.Parameters.AddWithValue("@pcedula", NpgsqlDbType.Bigint).Value = pUser._cedula;
                command.Parameters.AddWithValue("@pcontrasena", NpgsqlDbType.Text).Value = pUser._contrasena;
                command.Parameters.AddWithValue("@ptelefono", NpgsqlDbType.Text).Value = pUser._telefono;

                //Si el valor del atributo "_codigo" es el valor por defecto, se trata de un usuario general o administrador
                if(pUser._codigo.Equals("")) { command.Parameters.AddWithValue("@pcodigo", NpgsqlDbType.Text).Value = DBNull.Value; }
                else { command.Parameters.AddWithValue("@pcodigo", NpgsqlDbType.Text).Value = pUser._codigo; }

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



        /// <summary>
        /// Método que permite a un usuario iniciar sesión en la página web
        /// </summary>
        /// <param name="pUser"></param>
        /// <returns></returns>
        [Route("login")]
        [HttpPost]
        public IHttpActionResult login(Usuario pUser) {

            List<Rol> _roles = new List<Rol>();

            using (NpgsqlConnection connection = DataBase.getConnection())
            {
                NpgsqlCommand command = new NpgsqlCommand("iniciarsesion", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@pusuario", NpgsqlDbType.Text).Value = pUser._usuario;
                command.Parameters.AddWithValue("@pcontrasena", NpgsqlDbType.Text).Value = pUser._contrasena;

                try
                {
                    connection.Open();
                    NpgsqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        if (reader.GetString(0) == Constants.ERROR_USER_PASS) break;
                        Rol rol = new Rol();
                        rol._role = reader.GetString(0);
                        _roles.Add(rol);
                    }

                    if (_roles.Count < 1) { return Json(new Response(Constants.ERROR_USER_PASS)); }
                    else { return Json(_roles); }

                }
                catch (NpgsqlException ex) { return Json(new Response(Constants.ERROR_DATABASE_CONNECTION)); }
                finally { connection.Close(); }
            }

        }





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
                command.CommandType = CommandType.StoredProcedure;

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
