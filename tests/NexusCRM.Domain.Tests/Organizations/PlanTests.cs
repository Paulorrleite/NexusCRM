using NexusCRM.Domain;
using NexusCRM.Domain.Organizations;

namespace NexusCRM.Domain.Tests.Organizations;

public sealed class PlanTests
{
    [Fact]
    public void Create_stores_limits_and_normalized_features()
    {
        var limits = new PlanLimits(
            maximumUsers: 3,
            maximumContacts: 100,
            maximumCompanies: 25,
            maximumOpportunities: 50);

        var plan = Plan.Create(
            "Free",
            "Free plan",
            price: 0,
            BillingPeriod.Monthly,
            limits,
            [" Contacts.Read ", "Companies.Create"]);

        Assert.NotEqual(Guid.Empty, plan.Id);
        Assert.Equal("Free", plan.Name);
        Assert.Equal(limits, plan.Limits);
        Assert.True(plan.IsActive);
        Assert.True(plan.AllowsFeature("contacts.read"));
        Assert.True(plan.AllowsFeature("COMPANIES.CREATE"));
    }

    [Fact]
    public void Create_rejects_negative_price()
    {
        var limits = new PlanLimits(1, 1, 1, 1);

        Assert.Throws<DomainException>(() =>
            Plan.Create("Free", "Free plan", -1, BillingPeriod.Monthly, limits, []));
    }

    [Fact]
    public void PlanLimits_rejects_zero_values()
    {
        Assert.Throws<DomainException>(() => new PlanLimits(0, 1, 1, 1));
    }
}
