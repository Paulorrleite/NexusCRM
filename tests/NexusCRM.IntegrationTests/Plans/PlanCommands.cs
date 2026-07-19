using NexusCRM.Application.Plans.EditPlan;
using NexusCRM.Application.Plans.RegisterPlan;
using NexusCRM.Domain.Organizations;

namespace NexusCRM.IntegrationTests.Plans;

internal static class PlanCommands
{
    public static RegisterPlanCommand Register(
        string name,
        string description,
        BillingPeriod billingPeriod = BillingPeriod.Monthly)
    {
        return new RegisterPlanCommand(
            name,
            description,
            billingPeriod == BillingPeriod.Monthly ? 49 : 490,
            billingPeriod,
            MaximumUsers: 10,
            MaximumContacts: 1_000,
            MaximumCompanies: 100,
            MaximumOpportunities: 500,
            ["Contacts.Read", "Companies.Create"]);
    }

    public static EditPlanCommand Edit(
        Guid planId,
        string name,
        string description)
    {
        return new EditPlanCommand(
            planId,
            name,
            description,
            Price: 99,
            BillingPeriod.Yearly,
            MaximumUsers: 20,
            MaximumContacts: 2_000,
            MaximumCompanies: 200,
            MaximumOpportunities: 1_000,
            ["Contacts.Read", "Companies.Create", "Opportunities.Create"],
            IsActive: true);
    }
}
