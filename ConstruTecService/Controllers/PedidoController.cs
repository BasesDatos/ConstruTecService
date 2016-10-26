using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Npgsql;
using NpgsqlTypes;
using ConstruTecService.Models;
using System.Data;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text;
using System.Diagnostics;

namespace ConstruTecService.Controllers
{
    [RoutePrefix("orders")]
    public class PedidoController : ApiController
    {

        [Route("register")]
        [HttpPost]
        public IHttpActionResult register(Pedido pPedido)
        {
            PedidoEPATEC order = new PedidoEPATEC();//this.getOrderDetails(pPedido._idRelacionEtapa);
            this.getOrderDetails(pPedido._idRelacionEtapa, order);
            order._fechaPedido = pPedido._fechaPedido;
            order._horaPedido = pPedido._horaPedido;

            using (NpgsqlConnection connection = DataBase.getConnection())
            {
                NpgsqlCommand command = new NpgsqlCommand("obtenermaterialesetapa", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@pidrelacion", NpgsqlDbType.Integer).Value = pPedido._idRelacionEtapa;

                try
                {
                    connection.Open();
                    order._productos = new List<Material>();
                    NpgsqlDataReader reader = command.ExecuteReader();
                    int counter = 0;
                    while (reader.Read())
                    {
                        Material material = new Material();
                        material._id = reader.GetInt64(0);
                        material._nombre = reader.GetString(1);
                        material._precio = reader.GetDecimal(2);
                        material._cantidadDisponible = reader.GetInt32(3);
                        order._productos.Add(material);
                        counter++;
                    }
                    if (counter > 0)
                    {
                        this.postOnEPATEC(order);
                        EtapaController controller = new EtapaController(); //ARREGLAR ESTA PETICION!!!!!!!!
                        return controller.endStage(pPedido._idRelacionEtapa);
                    }
                    else return Json(new Response("El pedido no se logro completar"));
                    
                }
                catch (NpgsqlException ex) { return Json(new Response(ex.Message)); }
                finally { connection.Close(); }
            }
        }

   


        /// <summary>
        /// Método que serializa la clase que contiene la información del pedido, y lo envía al
        /// web service de EPATEC, para que se registre en la base de datos
        /// </summary>
        /// <param name="pPedido"></param>
        public void postOnEPATEC(PedidoEPATEC pPedido)
        {

            //Se parsea el JSON obtenido a una lista de clases Material
            /*DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(PedidoEPATEC));

            var stream = new MemoryStream();
            serializer.WriteObject(stream, pPedido);

            stream.Position = 0;
            StreamReader sr = new StreamReader(stream);*/

            var stringPayload = JsonConvert.SerializeObject(pPedido);
            StringContent content = new StringContent(stringPayload, Encoding.UTF8, "application/json");

            using (var httpClient = new HttpClient())
            {
                // Do the actual request and await the response
                var httpResponse = httpClient.PostAsync("http://localhost:8080/pedidos/register", content);

                if (httpResponse.Result != null)
                {
                    var responseContent = httpResponse.Result.Content;
                }
            }

            /*string jsonPedido = sr.ReadToEnd();

            var syncClient = new WebClient();
            var content = syncClient.UploadString("http://localhost:8080/pedidos/register", jsonPedido);

            Debug.WriteLine(jsonPedido);*/
            }




        public void getOrderDetails(int pId, PedidoEPATEC pPedido)
        {
            using (NpgsqlConnection connection = DataBase.getConnection())
            {
                NpgsqlCommand command = new NpgsqlCommand("obtenerdetalles", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@pidrelacion", NpgsqlDbType.Integer).Value = pId;

                try
                {
                    connection.Open();
                    NpgsqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        pPedido._total = reader.GetDecimal(0);
                    }
                }
                catch (NpgsqlException ex) { }
                finally { connection.Close(); }
            }
        }



    }    
}
