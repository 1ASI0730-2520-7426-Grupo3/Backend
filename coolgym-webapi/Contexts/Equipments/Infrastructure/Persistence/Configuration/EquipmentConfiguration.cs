using coolgym_webapi.Contexts.Equipments.Domain;
using coolgym_webapi.Contexts.Equipments.Domain.Constants;
using coolgym_webapi.Contexts.Equipments.Domain.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace coolgym_webapi.Contexts.Equipments.Infrastructure.Persistence.Configuration;

/// <summary>
///     Entity Framework configuration for Equipment entity
///     Implements IEntityTypeConfiguration to separate configuration from DbContext
/// </summary>
public class EquipmentConfiguration : IEntityTypeConfiguration<Equipment>
{
    public void Configure(EntityTypeBuilder<Equipment> builder)
    {
        builder.ToTable("equipments");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(e => e.CreatedDate)
            .HasColumnName("created_date")
            .IsRequired();

        builder.Property(e => e.UpdatedDate)
            .HasColumnName("updated_date")
            .IsRequired(false);

        builder.Property(e => e.IsDeleted)
            .HasColumnName("is_deleted")
            .IsRequired()
            .HasDefaultValue(EquipmentDomainConstants.DeletedFlagFalse);

        builder.Property(e => e.Name)
            .HasColumnName("name")
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.Type)
            .HasColumnName("type")
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.Model)
            .HasColumnName("model")
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.Manufacturer)
            .HasColumnName("manufacturer")
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.Code)
            .HasColumnName("code")
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(e => e.SerialNumber)
            .HasColumnName("serial_number")
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.InstallationDate)
            .HasColumnName("installation_date")
            .IsRequired();

        builder.Property(e => e.PowerWatts)
            .HasColumnName("power_watts")
            .IsRequired();

        builder.Property(e => e.Status)
            .HasColumnName("status")
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.IsPoweredOn)
            .HasColumnName("is_powered_on")
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(e => e.ActiveStatus)
            .HasColumnName("active_status")
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.Notes)
            .HasColumnName("notes")
            .HasMaxLength(500)
            .IsRequired(false);

        builder.Property(e => e.Image)
            .HasColumnName("image")
            .HasMaxLength(500)
            .IsRequired(false);

        builder.HasIndex(e => e.SerialNumber)
            .IsUnique()
            .HasDatabaseName("ix_equipments_serial_number");

        ConfigureLocationValueObject(builder);
        ConfigureUsageValueObject(builder);
        ConfigureControlsValueObject(builder);
        ConfigureMaintenanceValueObject(builder);
    }

    /// <summary>
    ///     Configures Location Value Object (Equipment location)
    /// </summary>
    private static void ConfigureLocationValueObject(EntityTypeBuilder<Equipment> builder)
    {
        builder.OwnsOne(e => e.Location, location =>
        {
            location.Property(l => l.Name)
                .HasColumnName("location_name")
                .HasMaxLength(EquipmentDomainConstants.MaxLocationNameLength)
                .IsRequired();

            location.Property(l => l.Address)
                .HasColumnName("location_address")
                .HasMaxLength(EquipmentDomainConstants.MaxLocationAddressLength)
                .IsRequired();
        });
    }

    /// <summary>
    ///     Configures UsageStats Value Object (Usage statistics)
    /// </summary>
    private static void ConfigureUsageValueObject(EntityTypeBuilder<Equipment> builder)
    {
        builder.OwnsOne(e => e.Usage, usage =>
        {
            usage.Property(u => u.TotalMinutes)
                .HasColumnName("usage_total_minutes")
                .IsRequired();

            usage.Property(u => u.TodayMinutes)
                .HasColumnName("usage_today_minutes")
                .IsRequired();

            usage.Property(u => u.CaloriesToday)
                .HasColumnName("usage_calories_today")
                .IsRequired();
        });
    }

    /// <summary>
    ///     Configures ControlSettings Value Object (Controls and configuration)
    /// </summary>
    private static void ConfigureControlsValueObject(EntityTypeBuilder<Equipment> builder)
    {
        builder.OwnsOne(e => e.Controls, controls =>
        {
            controls.Property(c => c.Power)
                .HasColumnName("control_power")
                .HasMaxLength(10)
                .IsRequired();

            controls.Property(c => c.CurrentLevel)
                .HasColumnName("control_current_level")
                .IsRequired();

            controls.Property(c => c.SetLevel)
                .HasColumnName("control_set_level")
                .IsRequired();

            controls.Property(c => c.MinLevelRange)
                .HasColumnName("control_min_level_range")
                .IsRequired();

            controls.Property(c => c.MaxLevelRange)
                .HasColumnName("control_max_level_range")
                .IsRequired();

            controls.Property(c => c.Status)
                .HasColumnName("control_status")
                .HasMaxLength(50)
                .IsRequired();
        });
    }

    /// <summary>
    ///     Configures MaintenanceInfo Value Object (Maintenance information)
    /// </summary>
    private static void ConfigureMaintenanceValueObject(EntityTypeBuilder<Equipment> builder)
    {
        builder.OwnsOne(e => e.MaintenanceInfo, maintenance =>
        {
            maintenance.Property(m => m.LastMaintenanceDate)
                .HasColumnName("maintenance_last_date")
                .IsRequired(false);

            maintenance.Property(m => m.NextMaintenanceDate)
                .HasColumnName("maintenance_next_date")
                .IsRequired(false);
        });
    }
}
