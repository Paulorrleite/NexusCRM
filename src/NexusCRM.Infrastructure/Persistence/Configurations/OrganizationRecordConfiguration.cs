using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexusCRM.Infrastructure.Persistence.Models;

namespace NexusCRM.Infrastructure.Persistence.Configurations;

internal sealed class OrganizationRecordConfiguration : IEntityTypeConfiguration<OrganizationRecord>
{
    public void Configure(EntityTypeBuilder<OrganizationRecord> builder)
    {
        builder.ToTable("organizations");

        builder.HasKey(organization => organization.Id);

        builder.Property(organization => organization.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(organization => organization.Name)
            .HasColumnName("name")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(organization => organization.Slug)
            .HasColumnName("slug")
            .HasMaxLength(200)
            .IsRequired();

        builder.HasIndex(organization => organization.Slug)
            .IsUnique();

        builder.Property(organization => organization.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(organization => organization.CreatedAt)
            .HasColumnName("created_at");

        builder.Property(organization => organization.UpdatedAt)
            .HasColumnName("updated_at");
    }
}
