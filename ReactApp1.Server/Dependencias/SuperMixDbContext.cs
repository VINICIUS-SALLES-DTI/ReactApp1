using Microsoft.EntityFrameworkCore;
using ReactApp1.Server.Apresentacao.Dependencias.Persistencia.Entidades;

namespace ReactApp1.Server.Apresentacao.Dependencias;

public class SuperMixDbContext : DbContext
{
    public SuperMixDbContext(DbContextOptions<SuperMixDbContext> options) : base(options)
    {
    }

    public DbSet<Material> Materiais { get; set; }
    public DbSet<Traco> Tracos { get; set; }
    public DbSet<TracoMaterial> TracoMateriais { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TracoMaterial>()
            .HasKey(tm => new { tm.TracoId, tm.MaterialId });

        // Configuração opcional para definir os relacionamentos (EF Core geralmente infere, mas é bom ser explícito)
        modelBuilder.Entity<TracoMaterial>()
            .HasOne(tm => tm.Traco)
            .WithMany(t => t.TracoMateriais)
            .HasForeignKey(tm => tm.TracoId);

        modelBuilder.Entity<TracoMaterial>()
            .HasOne(tm => tm.Material)
            .WithMany(m => m.TracoMateriais)
            .HasForeignKey(tm => tm.MaterialId);
    }
}
