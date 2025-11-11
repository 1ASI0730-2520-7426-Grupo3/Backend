using coolgym_webapi.Contexts.BillingInvoices.Domain.Model.Entities;
using coolgym_webapi.Contexts.BillingInvoices.Domain.Model.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace coolgym_webapi.Contexts.BillingInvoices.Infrastructure.Persistence.Configuration;

/// <summary>
/// Entity Framework Core configuration for BillingInvoice entity
/// Maps domain model to database schema
/// </summary>
public class BillingInvoiceConfiguration : IEntityTypeConfiguration<BillingInvoice>
{
    public void Configure(EntityTypeBuilder<BillingInvoice> builder)
    {
        // Table name
        builder.ToTable("billing_invoices");

        // Primary key
        builder.HasKey(i => i.Id);
        builder.Property(i => i.Id)
            .HasColumnName("Id")
            .IsRequired();

        // UserId
        builder.Property(i => i.UserId)
            .HasColumnName("UserId")
            .IsRequired();

        // CompanyName
        builder.Property(i => i.CompanyName)
            .HasColumnName("CompanyName")
            .HasMaxLength(255)
            .IsRequired();

        // Money Value Object (OwnsOne pattern)
        builder.OwnsOne(i => i.Amount, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("Amount")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            money.Property(m => m.Currency)
                .HasColumnName("Currency")
                .HasMaxLength(3)
                .IsRequired();
        });

        // InvoiceStatus Value Object (OwnsOne pattern)
        builder.OwnsOne(i => i.Status, status =>
        {
            status.Property(s => s.Value)
                .HasColumnName("Status")
                .HasMaxLength(20)
                .IsRequired();
        });

        // IssuedAt
        builder.Property(i => i.IssuedAt)
            .HasColumnName("IssuedAt")
            .HasColumnType("datetime")
            .IsRequired();

        // PaidAt (nullable)
        builder.Property(i => i.PaidAt)
            .HasColumnName("PaidAt")
            .HasColumnType("datetime")
            .IsRequired(false);

        // BaseEntity properties
        builder.Property(i => i.CreatedDate)
            .HasColumnName("CreatedDate")
            .HasColumnType("datetime")
            .IsRequired();

        builder.Property(i => i.UpdatedDate)
            .HasColumnName("UpdatedDate")
            .HasColumnType("datetime")
            .IsRequired(false);

        builder.Property(i => i.IsDeleted)
            .HasColumnName("IsDeleted")
            .HasDefaultValue(0)
            .IsRequired();

        // Indexes for performance
        builder.HasIndex(i => i.UserId);
        builder.HasIndex(i => i.IssuedAt);
    }
}
