using coolgym_webapi.Contexts.Security.Domain.Model;
using coolgym_webapi.Contexts.Security.Domain.Model.Entities;
using coolgym_webapi.Contexts.Security.Domain.Model.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace coolgym_webapi.Contexts.Security.Infrastructure.Persistence.Configuration;

/// <summary>
///     Entity Framework configuration for User entity
/// </summary>
public class UserEntityConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id)
            .ValueGeneratedOnAdd();

        builder.Property(u => u.CreatedDate)
            .IsRequired();

        builder.Property(u => u.UpdatedDate)
            .IsRequired(false);

        builder.Property(u => u.IsDeleted)
            .IsRequired()
            .HasDefaultValue(SecurityDomainConstants.DeletedFlagFalse);

        builder.Property(u => u.Username)
            .IsRequired()
            .HasMaxLength(SecurityDomainConstants.MaxUsernameLength);

        builder.Property(u => u.PasswordHash)
            .IsRequired();

        builder.Property(u => u.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(u => u.Phone)
            .HasMaxLength(20);

        builder.Property(u => u.Type)
            .IsRequired()
            .HasMaxLength(50);

        // Configure Role enum to be stored as string in database
        builder.Property(u => u.Role)
            .IsRequired()
            .HasConversion(
                v => v.ToRoleName(), // To database: UserRole → "Client"/"Provider"
                v => UserRoleExtensions.FromString(v) // From database: "Client"/"Provider" → UserRole
            )
            .HasMaxLength(50);

        builder.Property(u => u.ClientPlanId)
            .IsRequired(false);

        builder.Property(u => u.ProfilePhoto)
            .HasColumnType("LONGTEXT")
            .IsRequired(false);

        // Configure Email value object
        builder.OwnsOne(u => u.Email, email =>
        {
            email.Property(e => e.Value)
                .HasColumnName("Email")
                .IsRequired()
                .HasMaxLength(255);

            // Create unique index on Email column
            email.HasIndex(e => e.Value)
                .IsUnique();
        });

        // Unique index on Username
        builder.HasIndex(u => u.Username)
            .IsUnique();
    }
}