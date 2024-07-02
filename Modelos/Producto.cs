using System;
using System.ComponentModel.DataAnnotations;

namespace webapi.Modelos
{
    public class Producto
    {
        public Int64 Seq { get; set; }
        [Key]
        public string U_TIENDA { get; set; }
        public int EXISTENCIA { get; set; }
        public string CODIGO { get; set; }
        public string TIENDA { get; set; }
        public string PRODUCTO { get; set; }
        public int necesitaReceta { get; set; }
        public string LABORATORIO { get; set; }
        public string Nivel1 { get; set; }
        public string Nivel2 { get; set; }
        public string Nivel3 { get; set; }
        public string Nivel4 { get; set; }
        public decimal PRECIO { get; set; }
        public Int64 IDPRODUCTO { get; set; }
        public string IMAGEN { get; set; }
        public decimal DESCUENTO { get; set; }
        public decimal PRECIOBRUTO { get; set; }
        public decimal OFERTA { get; set; }
        public decimal VALOROFERTA { get; set; }
        public string Estado { get; set; }
        public DateTime FechaHoraCambio { get; set; }
    }
}
