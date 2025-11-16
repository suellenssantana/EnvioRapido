using EnvioRapido.Api.Domain;
using Microsoft.EntityFrameworkCore;


namespace EnvioRapido.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Envio> Envios => Set<Envio>();
}
