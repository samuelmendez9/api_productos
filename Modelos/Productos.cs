using System;
using System.ComponentModel.DataAnnotations;


namespace webapi_ENVIA.Modelos
{
    public class Productos
    {
        public Int64 Seq { get; set; }
        [Key]
        public int IDPRODUCTO { get; set; }

        public string CODIGO { get; set; }

        public string PRODUCTO { get; set; }

        public decimal PRECIO { get; set; }

        public string LABORATORIO { get; set; }

        public string IMAGEN { get; set; }

        public int EXISTENCIA { get; set; }

        public string Nivel1 { get; set; }

        public string Nivel2 { get; set; }

        public string Nivel3 { get; set; }

        public string Nivel4 { get; set; }

        public string Principio_Activo { get; set; }

        public string Codigo_SAP { get; set; }

        public decimal PRECIO_BRUTO { get; set; }

        public decimal PORCENTAJE_DESCUENTO { get; set; }

        public decimal DESCUENTO { get; set; }

    }
}
