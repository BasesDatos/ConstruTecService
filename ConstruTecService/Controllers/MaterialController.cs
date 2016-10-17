using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ConstruTecService.Models;
using System.Web.Http.Results;
using System.Runtime.Serialization.Json;
using System.Text;

namespace ConstruTecService.Controllers
{
    [RoutePrefix("materials")]
    public class MaterialController : ApiController
    {

        /// <summary>
        /// Método que se comunica con el web service de EPATEC para obtener el listado de todos los productos 
        /// disponibles
        /// </summary>
        /// <returns></returns>
        [Route("getMaterials")]
        [HttpGet]
        public IHttpActionResult getMaterials()
        {
            
            var syncClient = new WebClient();
            var content = syncClient.DownloadString(Constants.URL_EPATEC_SERVICE);

            //Se parsea el JSON obtenido a una lista de clases Material
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(List<Material>));
            var materials = new List<Material>();
            using (var stream = new MemoryStream(Encoding.Unicode.GetBytes(content)))
            {
                materials = (List<Material>)serializer.ReadObject(stream);
            }

            if(materials.Count < 1) { return Json(new Response(Constants.ERROR_SQLSERVER_CONNECTION)); }

            return Json(materials);
        }
    }
    
}
