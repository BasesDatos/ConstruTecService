using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ConstruTecService
{
    /// <summary>
    /// Clase utilizada para retornar algun mensaje a la página web o cliente que consume el web service
    /// </summary>
    public class Response
    {

        public string estado { get; set; }


        /// <summary>
        /// Constructor que recibe el mensaje
        /// </summary>
        /// <param name="pEstado"></param>
        public Response(string pEstado)
        {
            this.estado = pEstado;
        }
    }
}