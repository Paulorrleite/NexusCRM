using Microsoft.EntityFrameworkCore;
using NexusCRM.Application.Abstractions.Persistence;
using NexusCRM.Application.Plans;
using NexusCRM.Domain.Organizations;

namespace NexusCRM.Infrastructure.Persistence.Repositories;

internal sealed class PlanQueries(NexusCrmDbContext dbContext) : IPlanQueries
{
    public async Task<IReadOnlyCollection<PlanUsageOrganization>> GetOrganizationsUsingPlanAsync(
        Guid planId,
        CancellationToken cancellationToken)
    {
        return await (
                from subscription in dbContext.Subscriptions.AsNoTracking()
                join organization in dbContext.Organizations.AsNoTracking()
                    on subscription.OrganizationId equals organization.Id
                where subscription.PlanId == planId
                    && subscription.Status != SubscriptionStatus.Cancelled
                orderby organization.Name
                select new PlanUsageOrganization(
                    organization.Id,
                    organization.Name,
                    organization.Slug))
            .Distinct()
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<IReadOnlyCollection<PlanListItem>> ListAsync(
        string? name,
        string? description,
        BillingPeriod? billingPeriod,
        bool? isActive,
        CancellationToken cancellationToken)
    {
        var query = dbContext.Plans.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(name))
        {
            var normalizedName = name.Trim();
            query = query.Where(plan => EF.Functions.ILike(plan.Name, $"%{normalizedName}%"));
        }

        if (!string.IsNullOrWhiteSpace(description))
        {
            var normalizedDescription = description.Trim();
            query = query.Where(plan => EF.Functions.ILike(plan.Description, $"%{normalizedDescription}%"));
        }

        if (billingPeriod.HasValue)
        {
            query = query.Where(plan => plan.BillingPeriod == billingPeriod.Value);
        }

        if (isActive.HasValue)
        {
            query = query.Where(plan => plan.IsActive == isActive.Value);
        }

        return await query
            .OrderBy(plan => plan.Name)
            .Select(plan => new PlanListItem(
                plan.Id,
                plan.Name,
                plan.Description,
                plan.Price,
                plan.BillingPeriod,
                plan.IsActive))
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }
}
