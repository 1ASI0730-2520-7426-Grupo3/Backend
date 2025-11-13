using coolgym_webapi.Contexts.maintenance.Domain.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace coolgym_webapi.Contexts.maintenance.Infrastructure.Persistence.Configuration;

public class MaintenanceRequestConfiguration : IEntityTypeConfiguration<MaintenanceRequest>
{
    public void Configure(EntityTypeBuilder<MaintenanceRequest> builder)
    {
        // ===== TABLA =====
        builder.ToTable("maintenanceRequests");

        // ===== CLAVE PRIMARIA =====
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        // ===== CAMPOS HEREDADOS DE BASEENTITY =====
        builder.Property(e => e.CreatedDate)
            .HasColumnName("created_date")
            .IsRequired();

        builder.Property(e => e.UpdatedDate)
            .HasColumnName("updated_date")
            .IsRequired(false);
        
        builder.Property(e => e.IsDeleted)
            .HasColumnName("is_deleted")
            .IsRequired()
            .HasDefaultValue(0);
        
        
        // ===== CAMPOS ESPECÍFICOS DE MaintenanceRequest =====
        builder.Property(e => e.EquipmentId)
            .HasColumnName("equipment_id")
            .IsRequired();

        builder.Property(e => e.SelectedDate)
            .HasColumnName("selected_date")
            .IsRequired();

        builder.Property(e => e.Observation)
            .HasColumnName("observation")
            .IsRequired();

        builder.Property(e => e.Status)
            .HasColumnName("status")
            .IsRequired(false);
        
        
    }
    
}