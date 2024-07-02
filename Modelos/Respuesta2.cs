using System.Collections.Generic;

namespace webapi.Modelos
{
    public class Respuesta2 : RespuestaBase
    {
        public IEnumerable<Producto> productos { get; set; }
    }
}