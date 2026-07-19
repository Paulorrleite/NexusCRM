using Microsoft.EntityFrameworkCore;
using NexusCRM.Application.Abstractions.Persistence;
using NexusCRM.Domain.Organizations;
using NexusCRM.Infrastructure.Persistence.Models;

namespace NexusCRM.Infrastructure.Persistence.Repositories;

internal sealed class OrganizationRepository(NexusCrmDbContext dbContext) : IOrganizationRepository
{
    public async Task AddAsync(Organization organization, CancellationToken cancellationToken)
    {
        await dbContext.Organizations.AddAsync(
            OrganizationRecord.FromDomain(organization),
            cancellationToken).ConfigureAwait(false);
    }

    public async Task<Organization?> GetByIdAsync(Guid organizationId, CancellationToken cancellationToken)
    {
        var organization = await dbContext.Organizations
            .AsNoTracking()
            .SingleOrDefaultAsync(organization => organization.Id == organizationId, cancellationToken)
            .ConfigureAwait(false);

        return organization?.ToDomain();
    }

    public async Task<bool> SlugExistsAsync(string slug, CancellationToken cancellationToken)
    {
        return await dbContext.Organizations
            .AnyAsync(organization => organization.Slug == slug, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task UpdateAsync(Organization organization, CancellationToken cancellationToken)
    {
        var organizationRecord = await dbContext.Organizations
            .SingleAsync(record => record.Id == organization.Id, cancellationToken)
            .ConfigureAwait(false);

        organizationRecord.UpdateFromDomain(organization);
    }
}
