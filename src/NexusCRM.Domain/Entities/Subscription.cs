using NexusCRM.Domain;

namespace NexusCRM.Domain.Organizations;

public sealed class Subscription
{
    private Subscription(
        Guid id,
        Guid organizationId,
        Guid planId,
        SubscriptionStatus status,
        DateTimeOffset startedAt,
        DateTimeOffset currentPeriodStart,
        DateTimeOffset currentPeriodEnd,
        DateTimeOffset? cancelledAt)
    {
        Id = id;
        OrganizationId = organizationId;
        PlanId = planId;
        Status = status;
        StartedAt = startedAt;
        CurrentPeriodStart = currentPeriodStart;
        CurrentPeriodEnd = currentPeriodEnd;
        CancelledAt = cancelledAt;
    }

    public Guid Id { get; }

    public Guid OrganizationId { get; }

    public Guid PlanId { get; private set; }

    public SubscriptionStatus Status { get; private set; }

    public DateTimeOffset StartedAt { get; }

    public DateTimeOffset CurrentPeriodStart { get; private set; }

    public DateTimeOffset CurrentPeriodEnd { get; private set; }

    public DateTimeOffset? CancelledAt { get; private set; }

    public bool IsActive => Status is SubscriptionStatus.Trialing or SubscriptionStatus.Active;

    public static Subscription Start(
        Guid organizationId,
        Guid planId,
        SubscriptionStatus status,
        DateTimeOffset startedAt,
        DateTimeOffset currentPeriodEnd)
    {
        EnsureValidId(organizationId, nameof(organizationId));
        EnsureValidId(planId, nameof(planId));

        if (status is SubscriptionStatus.Cancelled or SubscriptionStatus.PastDue)
        {
            throw new DomainException("A new subscription must start as trialing or active.");
        }

        if (currentPeriodEnd <= startedAt)
        {
            throw new DomainException("Subscription period end must be after the start date.");
        }

        return new Subscription(
            Guid.NewGuid(),
            organizationId,
            planId,
            status,
            startedAt,
            startedAt,
            currentPeriodEnd,
            cancelledAt: null);
    }

    public void ChangePlan(Guid planId, DateTimeOffset periodStart, DateTimeOffset periodEnd)
    {
        EnsureActive();
        EnsureValidId(planId, nameof(planId));

        if (periodEnd <= periodStart)
        {
            throw new DomainException("Subscription period end must be after the start date.");
        }

        PlanId = planId;
        CurrentPeriodStart = periodStart;
        CurrentPeriodEnd = periodEnd;
        Status = SubscriptionStatus.Active;
    }

    public void MarkPastDue()
    {
        EnsureNotCancelled();

        Status = SubscriptionStatus.PastDue;
    }

    public void Cancel(DateTimeOffset cancelledAt)
    {
        EnsureNotCancelled();

        Status = SubscriptionStatus.Cancelled;
        CancelledAt = cancelledAt;
    }

    private void EnsureActive()
    {
        if (!IsActive)
        {
            throw new DomainException("Only an active subscription can be changed.");
        }
    }

    private void EnsureNotCancelled()
    {
        if (Status == SubscriptionStatus.Cancelled)
        {
            throw new DomainException("The subscription is already cancelled.");
        }
    }

    private static void EnsureValidId(Guid id, string name)
    {
        if (id == Guid.Empty)
        {
            throw new DomainException($"{name} is required.");
        }
    }
}
