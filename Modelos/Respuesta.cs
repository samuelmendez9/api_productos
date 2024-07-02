using System.Collections.Generic;


namespace webapi_ENVIA.Modelos
{
    public class Respuesta
    {
        public string status { get; set; }
        public string mensaje { get; set; }

        public IEnumerable<Productos> productos { get; set; }

    }
}
