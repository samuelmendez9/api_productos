
using System.ComponentModel.DataAnnotations;


namespace webapi_ENVIA.Modelos
{
    public class Entrada
    {
        [Required]
        public string IDPEDIDO { get; set; }
        [Required]
        public string SERIE { get; set; }
        [Required]
        public string NUMERO { get; set; }
        public string ESTADO { get; set; }
        public string DESCRIBEESTADO { get; set; }
        public string DESCRIPCION { get; set; }
        [Required]
        public string CODIGO_UNICO_CLIENTE { get; set; }
        public string TIEMPO { get; set; }
        public string CODIGO_ENVIO { get; set; }
        public decimal? LATITUD { get; set; }
        public decimal? LONGITUD { get; set; }
    }
}
