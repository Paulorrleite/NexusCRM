using Microsoft.EntityFrameworkCore;
using NexusCRM.Application.Abstractions.Persistence;
using NexusCRM.Application.Organizations;
using NexusCRM.Domain.Organizations;
using NexusCRM.Infrastructure.Persistence.Models;

namespace NexusCRM.Infrastructure.Persistence.Repositories;

internal sealed class OrganizationQueries(NexusCrmDbContext dbContext) : IOrganizationQueries
{
    public async Task<IReadOnlyCollection<OrganizationListItem>> ListAsync(
        string? name,
        string? slug,
        OrganizationStatus? status,
        CancellationToken cancellationToken)
    {
        var query = dbContext.Organizations.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(name))
        {
            var normalizedName = name.Trim();
            query = query.Where(organization => EF.Functions.ILike(organization.Name, $"%{normalizedName}%"));
        }

        if (!string.IsNullOrWhiteSpace(slug))
        {
            var normalizedSlug = slug.Trim();
            query = query.Where(organization => EF.Functions.ILike(organization.Slug, $"%{normalizedSlug}%"));
        }

        if (status.HasValue)
        {
            query = query.Where(organization => organization.Status == status.Value);
        }

        return await ProjectListItem(query)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<IReadOnlyCollection<OrganizationListItem>> SearchAsync(
        string searchTerm,
        OrganizationStatus? status,
        CancellationToken cancellationToken)
    {
        var normalizedSearchTerm = searchTerm.Trim();
        var query = dbContext.Organizations
            .AsNoTracking()
            .Where(organization =>
                EF.Functions.ILike(organization.Name, $"%{normalizedSearchTerm}%")
                || EF.Functions.ILike(organization.Slug, $"%{normalizedSearchTerm}%"));

        if (status.HasValue)
        {
            query = query.Where(organization => organization.Status == status.Value);
        }

        return await ProjectListItem(query)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    private static IQueryable<OrganizationListItem> ProjectListItem(IQueryable<OrganizationRecord> query)
    {
        return query
            .OrderBy(organization => organization.Name)
            .Select(organization => new OrganizationListItem(
                organization.Id,
                organization.Name,
                organization.Slug,
                organization.Status,
                organization.CreatedAt,
                organization.UpdatedAt));
    }
}
