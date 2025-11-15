using coolgym_webapi.Contexts.BillingInvoices.Domain.Model.Entities;
using coolgym_webapi.Contexts.Equipments.Domain.Model.Entities;
using Microsoft.EntityFrameworkCore;

namespace coolgym_webapi.Contexts.Shared.Infrastructure.Persistence.Configuration;

public class CoolgymContext(DbContextOptions<CoolgymContext> options) : DbContext(options)
{
    // --- DbSets (Colecciones de Tablas) ---
    public DbSet<Equipment> Equipments { get; set; }
    public DbSet<BillingInvoice> BillingInvoices { get; set; }
    
    public DbSet<Contexts.RentalCatalog.Domain.Model.Entities.RentalItem> RentalItems { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Aplica TODAS las configuraciones IEntityTypeConfiguration automáticamente
        builder.ApplyConfigurationsFromAssembly(typeof(CoolgymContext).Assembly);
    }
    
}