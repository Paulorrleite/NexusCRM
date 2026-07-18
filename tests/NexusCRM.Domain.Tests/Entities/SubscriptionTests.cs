using NexusCRM.Domain.Organizations;

namespace NexusCRM.Domain.Tests.Entities;

public sealed class SubscriptionTests
{
    [Fact]
    public void Start_creates_active_subscription_period()
    {
        var startedAt = DateTimeOffset.UtcNow;
        var periodEnd = startedAt.AddMonths(1);
        var organizationId = Guid.NewGuid();
        var planId = Guid.NewGuid();

        var subscription = Subscription.Start(
            organizationId,
            planId,
            SubscriptionStatus.Active,
            startedAt,
            periodEnd);

        Assert.NotEqual(Guid.Empty, subscription.Id);
        Assert.Equal(organizationId, subscription.OrganizationId);
        Assert.Equal(planId, subscription.PlanId);
        Assert.Equal(SubscriptionStatus.Active, subscription.Status);
        Assert.Equal(startedAt, subscription.StartedAt);
        Assert.Equal(periodEnd, subscription.CurrentPeriodEnd);
        Assert.Null(subscription.CancelledAt);
    }

    [Fact]
    public void Start_rejects_invalid_initial_status()
    {
        var startedAt = DateTimeOffset.UtcNow;

        Assert.Throws<DomainException>(() =>
            Subscription.Start(
                Guid.NewGuid(),
                Guid.NewGuid(),
                SubscriptionStatus.PastDue,
                startedAt,
                startedAt.AddMonths(1)));
    }

    [Fact]
    public void ChangePlan_rejects_cancelled_subscription()
    {
        var startedAt = DateTimeOffset.UtcNow;
        var subscription = Subscription.Start(
            Guid.NewGuid(),
            Guid.NewGuid(),
            SubscriptionStatus.Active,
            startedAt,
            startedAt.AddMonths(1));

        subscription.Cancel(startedAt.AddDays(1));

        Assert.Throws<DomainException>(() =>
            subscription.ChangePlan(Guid.NewGuid(), startedAt.AddMonths(1), startedAt.AddMonths(2)));
    }
}
