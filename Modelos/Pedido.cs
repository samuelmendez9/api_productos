using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace webapi_ENVIA.Modelos
{
    [Table("OrdenEstatusEntregas", Schema = "TRACKING") ]
    public class Pedido
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        public Guid ordenId { get; set; }
        [Required]
        public Guid estatusEntregaId { get; set; }
        public byte estado { get; set; }
        public string CreatedBy { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastModified { get; set; }
        public string serieGenesis { get; set; }
        public string numeroGenesis { get; set; }
        public Guid? idPedidoFel { get; set; }
        public decimal? Latitud { get; set; }
        public decimal? Longitud { get; set; }

    }
}
