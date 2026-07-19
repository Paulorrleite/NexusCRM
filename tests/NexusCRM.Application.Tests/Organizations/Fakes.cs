using NexusCRM.Application.Abstractions.Persistence;
using NexusCRM.Application.Organizations;
using NexusCRM.Domain.Organizations;

namespace NexusCRM.Application.Tests.Organizations;

internal sealed class FakeOrganizationRepository : IOrganizationRepository
{
    public List<Organization> Organizations { get; } = [];

    public Guid? UpdatedOrganizationId { get; private set; }

    public HashSet<string> ExistingSlugs { get; } = new(StringComparer.OrdinalIgnoreCase);

    public Task AddAsync(Organization organization, CancellationToken cancellationToken)
    {
        Organizations.Add(organization);

        return Task.CompletedTask;
    }

    public Task<Organization?> GetByIdAsync(Guid organizationId, CancellationToken cancellationToken)
    {
        return Task.FromResult(Organizations.SingleOrDefault(organization => organization.Id == organizationId));
    }

    public Task<bool> SlugExistsAsync(string slug, CancellationToken cancellationToken)
    {
        return Task.FromResult(ExistingSlugs.Contains(slug));
    }

    public Task UpdateAsync(Organization organization, CancellationToken cancellationToken)
    {
        UpdatedOrganizationId = organization.Id;

        return Task.CompletedTask;
    }
}

internal sealed class FakeOrganizationQueries : IOrganizationQueries
{
    public List<OrganizationListItem> Organizations { get; } = [];

    public string? LastName { get; private set; }

    public string? LastSlug { get; private set; }

    public OrganizationStatus? LastStatus { get; private set; }

    public string? LastSearchTerm { get; private set; }

    public OrganizationStatus? LastSearchStatus { get; private set; }

    public Task<IReadOnlyCollection<OrganizationListItem>> ListAsync(
        string? name,
        string? slug,
        OrganizationStatus? status,
        CancellationToken cancellationToken)
    {
        LastName = name;
        LastSlug = slug;
        LastStatus = status;

        return Task.FromResult<IReadOnlyCollection<OrganizationListItem>>(Organizations);
    }

    public Task<IReadOnlyCollection<OrganizationListItem>> SearchAsync(
        string searchTerm,
        OrganizationStatus? status,
        CancellationToken cancellationToken)
    {
        LastSearchTerm = searchTerm;
        LastSearchStatus = status;

        return Task.FromResult<IReadOnlyCollection<OrganizationListItem>>(Organizations);
    }
}

internal sealed class FakeUnitOfWork : IUnitOfWork
{
    public int SaveChangesCallCount { get; private set; }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        SaveChangesCallCount++;

        return Task.FromResult(1);
    }
}
