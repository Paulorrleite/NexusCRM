using NexusCRM.Domain.Organizations;

namespace NexusCRM.Infrastructure.Persistence.Models;

internal sealed class SubscriptionRecord
{
    public Guid Id { get; set; }

    public Guid OrganizationId { get; set; }

    public Guid PlanId { get; set; }

    public SubscriptionStatus Status { get; set; }

    public DateTimeOffset StartedAt { get; set; }

    public DateTimeOffset CurrentPeriodStart { get; set; }

    public DateTimeOffset CurrentPeriodEnd { get; set; }

    public DateTimeOffset? CancelledAt { get; set; }
}
