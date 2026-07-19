using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexusCRM.Infrastructure.Persistence.Models;

namespace NexusCRM.Infrastructure.Persistence.Configurations;

internal sealed class PlanRecordConfiguration : IEntityTypeConfiguration<PlanRecord>
{
    public void Configure(EntityTypeBuilder<PlanRecord> builder)
    {
        builder.ToTable("plans");

        builder.HasKey(plan => plan.Id);

        builder.Property(plan => plan.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(plan => plan.Name)
            .HasColumnName("name")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(plan => plan.Description)
            .HasColumnName("description")
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(plan => plan.Price)
            .HasColumnName("price")
            .HasPrecision(18, 2);

        builder.Property(plan => plan.BillingPeriod)
            .HasColumnName("billing_period")
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(plan => plan.MaximumUsers)
            .HasColumnName("maximum_users");

        builder.Property(plan => plan.MaximumContacts)
            .HasColumnName("maximum_contacts");

        builder.Property(plan => plan.MaximumCompanies)
            .HasColumnName("maximum_companies");

        builder.Property(plan => plan.MaximumOpportunities)
            .HasColumnName("maximum_opportunities");

        builder.Property(plan => plan.IsActive)
            .HasColumnName("is_active");

        builder.Property(plan => plan.EnabledFeatures)
            .HasColumnName("enabled_features")
            .HasColumnType("text[]");
    }
}
