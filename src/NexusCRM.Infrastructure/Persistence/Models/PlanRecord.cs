using NexusCRM.Domain.Organizations;

namespace NexusCRM.Infrastructure.Persistence.Models;

internal sealed class PlanRecord
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public BillingPeriod BillingPeriod { get; set; }

    public int MaximumUsers { get; set; }

    public int MaximumContacts { get; set; }

    public int MaximumCompanies { get; set; }

    public int MaximumOpportunities { get; set; }

    public bool IsActive { get; set; }

    public string[] EnabledFeatures { get; set; } = [];

    public static PlanRecord FromDomain(Plan plan)
    {
        return new PlanRecord
        {
            Id = plan.Id,
            Name = plan.Name,
            Description = plan.Description,
            Price = plan.Price,
            BillingPeriod = plan.BillingPeriod,
            MaximumUsers = plan.Limits.MaximumUsers,
            MaximumContacts = plan.Limits.MaximumContacts,
            MaximumCompanies = plan.Limits.MaximumCompanies,
            MaximumOpportunities = plan.Limits.MaximumOpportunities,
            IsActive = plan.IsActive,
            EnabledFeatures = plan.EnabledFeatures.ToArray()
        };
    }
}
