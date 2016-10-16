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

        [Route("all_general")]
        [HttpGet]
        public IHttpActionResult allGeneral() {

            List<Usuario> _usuarios = new List<Usuario>();

            using (NpgsqlConnection connection = DataBase.getConnection())
            {
                NpgsqlCommand command = new NpgsqlCommand("listarUsuariosGenerales", connection);
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
                        //user._codigo = reader.GetString(6);
                        user._telefono = reader.GetString(6);
                        _usuarios.Add(user);
                    }

                    return Json(_usuarios);
                }
                catch(NpgsqlException ex) { return Json(new Respuesta("Error de conexión con la base de datos")); }
                finally { connection.Close(); }
            }


        }




    }
}
