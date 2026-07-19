using NexusCRM.Application.Organizations;
using NexusCRM.Application.Organizations.SearchOrganizations;
using NexusCRM.Domain.Organizations;

namespace NexusCRM.Application.Tests.Organizations;

public sealed class SearchOrganizationsQueryHandlerTests
{
    [Fact]
    public async Task Handle_returns_organizations_matching_search_term()
    {
        var organization = new OrganizationListItem(
            Guid.NewGuid(),
            "Reus Tecnologia",
            "reus-tecnologia",
            OrganizationStatus.Active,
            DateTimeOffset.UtcNow,
            DateTimeOffset.UtcNow);
        var organizationQueries = new FakeOrganizationQueries();
        organizationQueries.Organizations.Add(organization);
        var handler = new SearchOrganizationsQueryHandler(organizationQueries);

        var result = await handler.Handle(
            new SearchOrganizationsQuery("tecnologia", OrganizationStatus.Active),
            CancellationToken.None);

        Assert.Equal([organization], result);
        Assert.Equal("tecnologia", organizationQueries.LastSearchTerm);
        Assert.Equal(OrganizationStatus.Active, organizationQueries.LastSearchStatus);
    }
}
