using Microsoft.EntityFrameworkCore;
using NexusCRM.Application.Abstractions.Persistence;
using NexusCRM.Domain.Organizations;
using NexusCRM.Infrastructure.Persistence.Models;

namespace NexusCRM.Infrastructure.Persistence.Repositories;

internal sealed class PlanRepository(NexusCrmDbContext dbContext) : IPlanRepository
{
    public async Task AddAsync(Plan plan, CancellationToken cancellationToken)
    {
        await dbContext.Plans.AddAsync(
            PlanRecord.FromDomain(plan),
            cancellationToken).ConfigureAwait(false);
    }

    public async Task<bool> ExistsAsync(Guid planId, CancellationToken cancellationToken)
    {
        return await dbContext.Plans
            .AnyAsync(plan => plan.Id == planId, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task UpdateAsync(
        Guid planId,
        string name,
        string description,
        decimal price,
        BillingPeriod billingPeriod,
        PlanLimits limits,
        IReadOnlyCollection<string> enabledFeatures,
        bool isActive,
        CancellationToken cancellationToken)
    {
        var plan = await dbContext.Plans
            .SingleAsync(plan => plan.Id == planId, cancellationToken)
            .ConfigureAwait(false);

        plan.Name = name.Trim();
        plan.Description = description.Trim();
        plan.Price = price;
        plan.BillingPeriod = billingPeriod;
        plan.MaximumUsers = limits.MaximumUsers;
        plan.MaximumContacts = limits.MaximumContacts;
        plan.MaximumCompanies = limits.MaximumCompanies;
        plan.MaximumOpportunities = limits.MaximumOpportunities;
        plan.EnabledFeatures = enabledFeatures
            .Select(feature => feature.Trim().ToLowerInvariant())
            .Where(feature => feature.Length > 0)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
        plan.IsActive = isActive;
    }

    public async Task DeleteAsync(Guid planId, CancellationToken cancellationToken)
    {
        var plan = await dbContext.Plans
            .SingleAsync(plan => plan.Id == planId, cancellationToken)
            .ConfigureAwait(false);

        dbContext.Plans.Remove(plan);
    }
}
