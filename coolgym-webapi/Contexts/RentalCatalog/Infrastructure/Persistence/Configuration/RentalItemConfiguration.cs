using coolgym_webapi.Contexts.RentalCatalog.Domain.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace coolgym_webapi.Contexts.RentalCatalog.Infrastructure.Persistence.Configuration;

public class RentalItemConfiguration : IEntityTypeConfiguration<RentalItem>
{
    public void Configure(EntityTypeBuilder<RentalItem> builder)
    {
        builder.ToTable("rental_items");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasColumnName("name").IsRequired().HasMaxLength(120);
        builder.Property(x => x.Type).HasColumnName("type").IsRequired().HasMaxLength(60);
        builder.Property(x => x.Model).HasColumnName("model").IsRequired().HasMaxLength(60);
        builder.Property(x => x.ImageUrl).HasColumnName("image_url").HasMaxLength(256);
        builder.Property(x => x.IsAvailable).HasColumnName("is_available");

        // BaseEntity audit/soft delete
        builder.Property(x => x.IsDeleted).HasColumnName("is_deleted");
        builder.Property(x => x.CreatedDate).HasColumnName("created_date");
        builder.Property(x => x.UpdatedDate).HasColumnName("updated_date");

        // Owned type Money
        builder.OwnsOne(x => x.MonthlyPrice, m =>
        {
            m.Property(p => p.Amount).HasColumnName("monthly_price_amount").HasColumnType("decimal(10,2)");
            m.Property(p => p.Currency).HasColumnName("monthly_price_currency").HasMaxLength(10);
        });

        builder.HasIndex(x => new { x.Name, x.Model }).IsUnique();
    }
}