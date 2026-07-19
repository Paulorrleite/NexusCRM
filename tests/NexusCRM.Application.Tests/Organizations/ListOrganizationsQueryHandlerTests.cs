using NexusCRM.Application.Organizations;
using NexusCRM.Application.Organizations.ListOrganizations;
using NexusCRM.Domain.Organizations;

namespace NexusCRM.Application.Tests.Organizations;

public sealed class ListOrganizationsQueryHandlerTests
{
    [Fact]
    public async Task Handle_returns_organizations_from_query_contract()
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
        var handler = new ListOrganizationsQueryHandler(organizationQueries);

        var result = await handler.Handle(
            new ListOrganizationsQuery("reus", "reus", OrganizationStatus.Active),
            CancellationToken.None);

        Assert.Equal([organization], result);
        Assert.Equal("reus", organizationQueries.LastName);
        Assert.Equal("reus", organizationQueries.LastSlug);
        Assert.Equal(OrganizationStatus.Active, organizationQueries.LastStatus);
    }
}
