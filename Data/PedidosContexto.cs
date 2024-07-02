using Microsoft.EntityFrameworkCore;
using webapi_ENVIA.Modelos;

namespace webapi_ENVIA.Data
{
    public class PedidosContexto: DbContext
    {
        public PedidosContexto(DbContextOptions<PedidosContexto> options):base(options) {}

        public DbSet<Productos> ProductoItems { get; set; }
    }
}
