
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webapi.Data;
using webapi_ENVIA.Data;
using webapi_ENVIA.Modelos;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows;
using System.Net.Http;
using System.Text;
using webapi.Modelos;
using System.Text.Json;
using Microsoft.Data.SqlClient;

namespace webapi_ENVIA.Controllers
{
    [Authorize]
    [Route("api")]
    [ApiController]
    public class PedidosController : ControllerBase
    {
        private readonly PedidosContexto PedidosContext;
        private readonly PedidosContextoMK PedidosContextMK;
        //private readonly HttpClient httpClient = new HttpClient("http://192.200.9.229/wsproductos");
        private static readonly HttpClient client = new HttpClient();

        public PedidosController(PedidosContexto Pedidos, PedidosContextoMK PedidosMK)
        {
            PedidosContext = Pedidos;
            PedidosContextMK = PedidosMK;
        }

        [HttpGet("productos")]
        public async Task<Respuesta> GetProductos([Required(ErrorMessage = "El Parametro de {0} es requerido.")] string Cadena, string FiltroProducto)
        {
            string CadenaSQL = "";

            List<Productos> Productos = null;
            FiltroProducto = FiltroProducto.Replace(' ', '%');

            // Para Farmacias Cruz Verde
            if (Cadena == "1")
            {

                CadenaSQL = "SELECT ROW_NUMBER() OVER (ORDER BY P.ITEMCODE) AS Seq,  P.ITEMCODE IDPRODUCTO, P.ITEMCODE_SUSTITUTO CODIGO, P.ITEMNAME PRODUCTO, " +
                        "(CAST((PT.PRICE*((100 - PT.PRCDESCNORMAL)*0.01)) as Decimal (18,2)) - CAST((PT.PRICE*((100 - PT.PRCDESCNORMAL)*0.01)* (ISNULL(PT.PRCOFERTA,0) * 0.01)) AS decimal (18,2))) PRECIO, LAB.NOMBRE LABORATORIO, '' IMAGEN, 0 EXISTENCIA,  " +
                        "ISNULL(PTS.Nivel1,'') Nivel1, ISNULL(PTS.Nivel2,'') Nivel2, ISNULL(PTS.Nivel3,'') Nivel3, ISNULL(PTS.Nivel4,'') Nivel4, ISNULL(PTS.U_PrinActivo,'') Principio_Activo, P.ITEMCODE_SUSTITUTO Codigo_SAP,  " +
                        "CAST((PT.PRICE*((100 - PT.PRCDESCNORMAL)*0.01)) as Decimal (18,2)) PRECIO_BRUTO, " +
                        "ISNULL(PT.PRCOFERTA,0)  PORCENTAJE_DESCUENTO,  " +
                        "CAST(((PT.PRICE*((100 - PT.PRCDESCNORMAL)*0.01)) * (ISNULL(PT.PRCOFERTA,0) * 0.01)) AS decimal (18,2)) DESCUENTO " +
                        "from PRODUCCION.TBL_PRODUCTOS_TIENDAS PT " +
                        "INNER JOIN PRODUCCION.TBL_PRODUCTOS P ON P.ITEMCODE = PT.ITEMCODE   " +
                        "INNER JOIN PRODUCCION.TBL_LABORATORIOS LAB ON P.U_LABORATORIO = LAB.U_LABORATORIO   " +
                        "LEFT JOIN PRODUCCION.TBL_PRODUCTOS_SAP PTS ON  PTS.ITEMCODE COLLATE Latin1_General_CI_AI = P.ITEMCODE_SUSTITUTO COLLATE Latin1_General_CI_AI " +
                        "WHERE PT.U_PAIS = 320 AND LAB.U_PAIS = 320 AND PT.U_TIENDA = 8  AND P.ESTADO = 1 AND P.APLICA_LP = 0 AND  " +
                        "(  " +
                        "RTRIM(P.ITEMCODE_SUSTITUTO)   + P.ITEMNAME  COLLATE Latin1_General_CI_AI  +  " +
                        "ISNULL(PTS.Nivel1,'') COLLATE Latin1_General_CI_AI  +  " +
                        "ISNULL(PTS.Nivel2,'') COLLATE Latin1_General_CI_AI  +  " +
                        "ISNULL(PTS.Nivel3,'') COLLATE Latin1_General_CI_AI  +  " +
                        "ISNULL(PTS.Nivel4,'') COLLATE Latin1_General_CI_AI  +  " +
                        "ISNULL(PTS.U_PrinActivo,'') COLLATE Latin1_General_CI_AI  +  " +
                        "ISNULL(LAB.NOMBRE,'') COLLATE Latin1_General_CI_AI  + P.ITEMNAME COLLATE Latin1_General_CI_AI  +  " +
                        "ISNULL(LAB.NOMBRE,'')  COLLATE Latin1_General_CI_AI  + P.ITEMNAME COLLATE Latin1_General_CI_AI  " +
                        ") LIKE '%" + FiltroProducto + "%' ";

                Productos = await PedidosContext.ProductoItems.FromSqlRaw(CadenaSQL).ToListAsync();

                //Console.Write("ANTES DE QUE ENTRE");

                Productos.ForEach(x => {
                    string ItemCodeSustituto = x.CODIGO.ToString();
                    x.IMAGEN = Conversor2(ItemCodeSustituto);

                    PedidosContext.ProductoItems.Update(x);

                    //Console.Write("TERMINO DE ENTRAR");

                });

                //await PedidosContext.SaveChangesAsync();
            }

            // Para Farmacias Meykos
            if (Cadena == "2")
            {
                CadenaSQL = "SELECT ROW_NUMBER() OVER (ORDER BY P.Codigo_PRODUCTO) AS Seq, CAST(P.Codigo_PRODUCTO as int) IDPRODUCTO, " +
                    "RTRIM(P.Codigo_PRODUCTO) CODIGO,  " +
                    "RTRIM(P.Descripcion_Producto) PRODUCTO, CAST(P.precio AS DECIMAL (18,2)) PRECIO,  " +
                    "RTRIM(P.Laboratorio) LABORATORIO, '' IMAGEN, 0 EXISTENCIA,  " +
                    "RTRIM(P.SubDepto) Nivel1,  " +
                    " '' as Nivel2,  " +
                    " '' as Nivel3,  " +
                    " '' as Nivel4,  " +
                    "p.Principio_Activo, " +
                    "RTRIM(P.Codigo_SAP) Codigo_SAP, " +
                    "CAST(P.PRECIO_BRUTO AS DECIMAL (18,2)) PRECIO_BRUTO , CAST(P.Porc_Descuento AS Decimal (18,2)) PORCENTAJE_DESCUENTO, Cast(P.DESCUENTO as Decimal (18,2)) DESCUENTO " +
                    "FROM vw_Catalogo_Productos P  " +
                    "WHERE P.estado = 'Activo' and ( " +
                    "RTRIM(ISNULL(P.Descripcion_Producto,'')) COLLATE Latin1_General_CI_AI + " +
                    "RTRIM(ISNULL(P.Codigo_Producto,'')) COLLATE Latin1_General_CI_AI + " +
                    "RTRIM(ISNULL(P.Laboratorio,'')) COLLATE Latin1_General_CI_AI + " +
                    "RTRIM(ISNULL(P.Subdepto,'')) COLLATE Latin1_General_CI_AI + " +
                    "RTRIM(ISNULL(P.Principio_Activo,'')) COLLATE Latin1_General_CI_AI ) like '%" + FiltroProducto + "%' ";

                Productos = await PedidosContextMK.ProductoItems.FromSqlRaw(CadenaSQL).ToListAsync();

                Productos.ForEach(x => {
                    string ItemCodeSustituto = x.Codigo_SAP.ToString();
                    x.IMAGEN = Conversor2(ItemCodeSustituto);
                    PedidosContextMK.ProductoItems.Update(x);
                });
            }

            // Para DERMACENTER
            

            //var Resultado = await PedidosContext.ProductoItems.FromSqlRaw(Cadena).ToListAsync();


            if (Productos.Count > 0)
            {
                return new Respuesta
                {
                    status = "1",
                    mensaje = "Se encontraron items relacionados",
                    productos = Productos
                };
            }
            else
            {
                return new Respuesta
                {
                    status = "0",
                    mensaje = "NO encontraron items relacionados",
                };
            }            
        }

        [HttpGet("productoslista")]
        public async Task<Respuesta> GetProductosporbloque([Required(ErrorMessage = "El Parametro de {0} es requerido.")] string Cadena, string FiltroProducto)
        {
            string CadenaSQL = "";

            List<Productos> Productos = null;
            //FiltroProducto = FiltroProducto.Replace(' ', '%');
            // Para Farmacias Cruz Verde
            if (Cadena == "1")
            {
                CadenaSQL = "SELECT ROW_NUMBER() OVER (ORDER BY P.ITEMCODE) AS Seq,  P.ITEMCODE IDPRODUCTO, P.ITEMCODE_SUSTITUTO CODIGO, P.ITEMNAME PRODUCTO, " +
                        "(CAST((PT.PRICE*((100 - PT.PRCDESCNORMAL)*0.01)) as Decimal (18,2)) - CAST((PT.PRICE*((100 - PT.PRCDESCNORMAL)*0.01)* (ISNULL(PT.PRCOFERTA,0) * 0.01)) AS decimal (18,2))) PRECIO, LAB.NOMBRE LABORATORIO, '' IMAGEN, 0 EXISTENCIA,  " +
                        "ISNULL(PTS.Nivel1,'') Nivel1, ISNULL(PTS.Nivel2,'') Nivel2, ISNULL(PTS.Nivel3,'') Nivel3, ISNULL(PTS.Nivel4,'') Nivel4, ISNULL(PTS.U_PrinActivo,'') Principio_Activo, P.ITEMCODE_SUSTITUTO Codigo_SAP,  " +
                        "CAST((PT.PRICE*((100 - PT.PRCDESCNORMAL)*0.01)) as Decimal (18,2)) PRECIO_BRUTO, " +
                        "ISNULL(PT.PRCOFERTA,0)  PORCENTAJE_DESCUENTO,  " +
                        "CAST(((PT.PRICE*((100 - PT.PRCDESCNORMAL)*0.01)) * (ISNULL(PT.PRCOFERTA,0) * 0.01)) AS decimal (18,2)) DESCUENTO " +
                        "from PRODUCCION.TBL_PRODUCTOS_TIENDAS PT " +
                        "INNER JOIN PRODUCCION.TBL_PRODUCTOS P ON P.ITEMCODE = PT.ITEMCODE   " +
                        "INNER JOIN PRODUCCION.TBL_LABORATORIOS LAB ON P.U_LABORATORIO = LAB.U_LABORATORIO   " +
                        "LEFT JOIN PRODUCCION.TBL_PRODUCTOS_SAP PTS ON  PTS.ITEMCODE COLLATE Latin1_General_CI_AI = P.ITEMCODE_SUSTITUTO COLLATE Latin1_General_CI_AI " +
                        "WHERE PT.U_PAIS = 320 AND LAB.U_PAIS = 320 AND PT.U_TIENDA = 8  AND P.ESTADO = 1 AND P.APLICA_LP = 0 AND  " +
                        "RTRIM(P.ITEMCODE_SUSTITUTO)  " +
                        " in (" + FiltroProducto + ") ";

                Productos = await PedidosContext.ProductoItems.FromSqlRaw(CadenaSQL).ToListAsync();

                //Console.Write("ANTES DE QUE ENTRE");

                //Productos.ForEach(x => {
                //    string ItemCodeSustituto = x.CODIGO.ToString();
                //    x.IMAGEN = Conversor2(ItemCodeSustituto);

                //    PedidosContext.ProductoItems.Update(x);
                //});
            }

            if (Productos.Count > 0)
            {
                return new Respuesta
                {
                    status = "1",
                    mensaje = "Se encontraron items relacionados",
                    productos = Productos
                };
            }
            else
            {
                return new Respuesta
                {
                    status = "0",
                    mensaje = "NO encontraron items relacionados",
                };
            }
        }

        private static string Conversor(string Valor, string Unidad, string Carpeta)
        {
            ImageConverter imgCon = new System.Drawing.ImageConverter();
            string base64ImageRepresentation = "";
            //var root = "mnt";
            //var folder = "imagenesProductos";
            //var file = ""+ Valor+".jpg";
            //var fullFileName = System.IO.Path.Combine(root, folder, file);

            try
            {
                //byte[] imageArray = System.IO.File.ReadAllBytes(@"C:\imagenesProductos\" + Valor + ".jpg");
                //byte[] imageArray = System.IO.File.ReadAllBytes(fullFileName);
                byte[] imageArray = System.IO.File.ReadAllBytes("/mnt/imagenesProductos/" + Valor + ".jpg");
                

                using (var ms = new MemoryStream(imageArray))
                {
                    //base64ImageRepresentation = Convert.ToBase64String((byte[])imgCon.ConvertTo(ResizeImage(Image.FromStream(ms), 400, 355), typeof(byte[])));
                    base64ImageRepresentation = Convert.ToBase64String((byte[])imgCon.ConvertTo(ResizeImage(Image.FromStream(ms), 944, 708), typeof(byte[])));
                }

                //return ;
                Console.WriteLine("lo encontro");
                return base64ImageRepresentation;
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message.ToString());
                return "";
            }
        }

        public string Conversor2(string Valor)
        {
            try
            {
                client.DefaultRequestHeaders.Accept.Clear();
                StringContent queryString = new StringContent($"Codigo_P={Valor}", Encoding.UTF8, "application/x-www-form-urlencoded");
               
                byte[] byteArray = Encoding.ASCII.GetBytes("Landingpage:ProyectoLandingpage2020");
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                HttpResponseMessage response = client.PostAsync("http://192.200.9.229/wsproductos/WsSegurosFCV.asmx/ImagenProductoBase64", queryString).GetAwaiter().GetResult();

                if (response.IsSuccessStatusCode)
                {

                    HttpContent content = response.Content;
                    var _serialize = content.ReadAsStringAsync().GetAwaiter().GetResult();
                    var _deserialize = JsonSerializer.Deserialize<RespuestaImagenBase64>(_serialize);

                    if (_deserialize == null)
                    {
                        return null;
                    }

                    return _deserialize.ImagenBase64;
                }

                return "";

            }
            catch (Exception ex)
            {
                Console.Write(ex.Message.ToString());
                return "";
            }
        }

        public static Image ResizeImage(Image srcImage, int newWidth, int newHeight)
        {
            using (Bitmap imagenBitmap =
               new Bitmap(newWidth, newHeight, PixelFormat.Format32bppRgb))
            {
                imagenBitmap.SetResolution(
                   Convert.ToInt32(srcImage.HorizontalResolution),
                   Convert.ToInt32(srcImage.HorizontalResolution));

                using (Graphics imagenGraphics =
                        Graphics.FromImage(imagenBitmap))
                {
                    imagenGraphics.SmoothingMode =
                       SmoothingMode.AntiAlias;
                    imagenGraphics.InterpolationMode =
                       InterpolationMode.HighQualityBicubic;
                    imagenGraphics.PixelOffsetMode =
                       PixelOffsetMode.HighQuality;
                    imagenGraphics.DrawImage(srcImage,
                       new Rectangle(0, 0, newWidth, newHeight),
                       new Rectangle(0, 0, srcImage.Width, srcImage.Height),
                       GraphicsUnit.Pixel);
                    MemoryStream imagenMemoryStream = new MemoryStream();
                    imagenBitmap.Save(imagenMemoryStream, ImageFormat.Jpeg);
                    srcImage = Image.FromStream(imagenMemoryStream);
                }
            }
            return srcImage;
        }

        [HttpGet("productoscambios")]
        public async Task<Respuesta2> GetProductosPorTiendaModificados([Required(ErrorMessage = "El Parametro de  {0} es requerido.")] string FechaInicio, string FechaFin)
        {
            string CadenaSQL = "";
            string iP = "", strSql = "";
            //Tienda Tiendas = null;
            //FiltroProducto = FiltroProducto.Replace(' ', '%');

            //strSql = "SELECT U_TIENDA, NOMBRE, DIRECCION, DIRECCIONIP FROM PRODUCCION.TBL_TIENDAS WHERE U_TIENDA = '" + Tienda + "' AND estado = 1 ";

            //Tiendas = await PedidosContext.TiendasItems.FromSqlRaw(strSql).SingleOrDefaultAsync();

            //Tiendas.ForEach(x => {
            //    string iP = x.Ip.ToString();
            //});
            //iP = Tiendas.DIRECCIONIP.ToString();
            iP = "Data Source=192.200.9.131;User Id=sa;password=bofasa1$; Pooling=false; Connect Timeout=1200;";

            string CONNECTION_STRING = iP;

            var productos = GetListaProductosModificados(iP, FechaInicio, FechaFin);

            return productos.Count > 0 ? new Respuesta2 { mensaje = "", productos = productos, status = "0" } :
                new Respuesta2 { mensaje = "No existen cambios en el rango seleccionado", productos = null, status = "1" };
        }


        private List<Producto> GetListaProductosModificados(string ip, string FechaInicio, string FechaFin)
        {
            List<Producto> inventoryList = new List<Producto>();
            //string connectionString = Configuration["ConnectionStrings:DefaultConnection"];
            string connectionString = ip;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string sql = "select T.U_TIENDA, EXISTENCIA, P.ITEMCODE_SUSTITUTO CODIGO, T.NOMBRE TIENDA, P.NOMBRECORTO PRODUCTO, " +
                " CASE WHEN P.U_RECETADO = 'Y' THEN 1 WHEN P.U_RECETADO = 'N' THEN 0 ELSE 0 END AS necesitaReceta, L.NOMBRE LABORATORIO, " +
                " (CAST((PT.PRICE*((100 - PT.PRCDESCNORMAL)*0.01)) as Decimal (18,2)) - CAST((PT.PRICE*((100 - PT.PRCDESCNORMAL)*0.01)* (ISNULL(PT.PRCOFERTA,0) * 0.01)) AS decimal (18,2))) PRECIO , P.ITEMCODE IDPRODUCTO, " +
                " PT.PRCDESCNORMAL as DESCUENTO, " +
                " PT.PRICE as PRECIOBRUTO, " +
                " isnull(PT.PRCOFERTA,0) as OFERTA, " +
                " ISNULL(PS.Nivel1,'') Nivel1, ISNULL(PS.Nivel2,'') Nivel2, ISNULL(PS.Nivel3,'') Nivel3, ISNULL(PS.Nivel4,'') Nivel4, " +
                " isnull(CAST(((PT.PRICE*((100 - PT.PRCDESCNORMAL)* 0.01)) - ((PT.PRICE*((100 - PT.PRCDESCNORMAL)* 0.01)) * ((100 - PT.PRCOFERTA)* 0.01))) AS DECIMAL(18, 2)),0) VALOROFERTA, " +
                " Pcc.Estado, Pcc.FECHA_MODIFICACION as FechaHoraCambio " +
                " from PRODUCCION.TBL_PRODUCTOS_TIENDAS PT " +
                " INNER JOIN PRODUCCION.TBL_PRODUCTOS P ON P.ITEMCODE = PT.ITEMCODE " +
                " INNER JOIN SERMESA.TBL_PRODUCTOS_CAMBIOS Pcc ON P.ITEMCODE = Pcc.ITEMCODE " +
                " INNER JOIN PRODUCCION.TBL_TIENDAS T ON PT.U_TIENDA = T.U_TIENDA " +
                " INNER JOIN PRODUCCION.TBL_LABORATORIOS L ON P.U_LABORATORIO = L.U_LABORATORIO " +
                " LEFT JOIN PRODUCCION.TBL_PRODUCTOS_SAP PS ON PS.prodcod = P.itemcode " +
                " WHERE PT.U_PAIS = 320 AND T.U_TIENDA = 73 and Pcc.FECHA_MODIFICACION between '" + FechaInicio + "' and '" + FechaFin + "' ";
                //" WHERE PT.U_PAIS = 320 AND T.U_TIENDA = 73 and Pcc.FECHA_MODIFICACION between '" + String.Format("{0:yyyyMMdd}", FechaInicio) + "' and '" + String.Format("{0:yyyyMMdd}", FechaFin) + "' ";

                SqlCommand command = new SqlCommand(sql, connection);

                using (SqlDataReader dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        Producto Producto = new Producto();

                        Producto.IDPRODUCTO = Convert.ToInt64(dataReader["IDPRODUCTO"].ToString());
                        Producto.CODIGO = dataReader["CODIGO"].ToString();
                        Producto.PRODUCTO = dataReader["PRODUCTO"].ToString();
                        Producto.U_TIENDA = dataReader["U_TIENDA"].ToString();
                        Producto.TIENDA = dataReader["TIENDA"].ToString();
                        Producto.PRECIO = Convert.ToDecimal(dataReader["PRECIO"]);
                        Producto.EXISTENCIA = Convert.ToInt32(dataReader["EXISTENCIA"]);
                        Producto.LABORATORIO = dataReader["LABORATORIO"].ToString();
                        Producto.Nivel1 = dataReader["Nivel1"].ToString();
                        Producto.Nivel2 = dataReader["Nivel2"].ToString();
                        Producto.Nivel3 = dataReader["Nivel3"].ToString();
                        Producto.Nivel4 = dataReader["Nivel4"].ToString();
                        Producto.Estado = dataReader["Estado"].ToString();
                        Producto.PRECIOBRUTO = Convert.ToDecimal(dataReader["PRECIOBRUTO"].ToString());
                        Producto.DESCUENTO = Convert.ToDecimal(dataReader["DESCUENTO"].ToString());
                        Producto.OFERTA = Convert.ToDecimal(dataReader["OFERTA"].ToString());
                        Producto.VALOROFERTA = Convert.ToDecimal(dataReader["VALOROFERTA"].ToString());
                        Producto.necesitaReceta = Convert.ToInt32(dataReader["necesitaReceta"].ToString());
                        Producto.FechaHoraCambio = Convert.ToDateTime(dataReader["FechaHoraCambio"].ToString());
                        inventoryList.Add(Producto);
                    }
                }

                connection.Close();
            }

            return inventoryList;
        }

    }
}
