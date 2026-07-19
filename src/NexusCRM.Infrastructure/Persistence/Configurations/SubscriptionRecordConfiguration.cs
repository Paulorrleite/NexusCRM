using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexusCRM.Infrastructure.Persistence.Models;

namespace NexusCRM.Infrastructure.Persistence.Configurations;

internal sealed class SubscriptionRecordConfiguration : IEntityTypeConfiguration<SubscriptionRecord>
{
    public void Configure(EntityTypeBuilder<SubscriptionRecord> builder)
    {
        builder.ToTable("subscriptions");

        builder.HasKey(subscription => subscription.Id);

        builder.Property(subscription => subscription.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(subscription => subscription.OrganizationId)
            .HasColumnName("organization_id");

        builder.Property(subscription => subscription.PlanId)
            .HasColumnName("plan_id");

        builder.Property(subscription => subscription.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(subscription => subscription.StartedAt)
            .HasColumnName("started_at");

        builder.Property(subscription => subscription.CurrentPeriodStart)
            .HasColumnName("current_period_start");

        builder.Property(subscription => subscription.CurrentPeriodEnd)
            .HasColumnName("current_period_end");

        builder.Property(subscription => subscription.CancelledAt)
            .HasColumnName("cancelled_at");

        builder.HasIndex(subscription => subscription.PlanId);
        builder.HasIndex(subscription => subscription.OrganizationId);
    }
}
