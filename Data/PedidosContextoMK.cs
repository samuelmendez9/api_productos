using Microsoft.EntityFrameworkCore;
using webapi_ENVIA.Modelos;

namespace webapi.Data
{
    public class PedidosContextoMK : DbContext
    {
        public PedidosContextoMK(DbContextOptions<PedidosContextoMK> options) : base(options)
        {

        }


        public DbSet<Productos> ProductoItems { get; set; }
    }
}
