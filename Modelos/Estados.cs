using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace webapi_ENVIA.Modelos
{
    [Table("EstatusEntregas", Schema = "CATALOGOS")]
    public class Estados
    {
        [Required]
        public Guid id { get; set; }
        [Required]
        public int codigoEstatus { get; set; }
        [Required]
        public string nombre { get; set; }
        public byte estado { get; set; }
    }
}
