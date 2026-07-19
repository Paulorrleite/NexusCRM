using MediatR;
using Microsoft.Extensions.DependencyInjection;
using NexusCRM.Application.Organizations.SearchOrganizations;
using NexusCRM.Domain.Organizations;
using NexusCRM.IntegrationTests.Infrastructure;

namespace NexusCRM.IntegrationTests.Organizations;

[Collection(IntegrationTestCollection.Name)]
public sealed class SearchOrganizationsIntegrationTests(IntegrationTestFixture fixture)
    : IntegrationTestBase(fixture)
{
    [Fact]
    public async Task SearchOrganizations_matches_name_or_slug()
    {
        using var scope = Fixture.CreateScope();
        var sender = scope.ServiceProvider.GetRequiredService<ISender>();

        await sender.Send(
            OrganizationCommands.Register("Reus Tecnologia"),
            CancellationToken.None);
        await sender.Send(
            OrganizationCommands.Register("Acme Sales"),
            CancellationToken.None);
        var suspended = await sender.Send(
            OrganizationCommands.Register("Tecnologia Suspensa"),
            CancellationToken.None);
        await sender.Send(
            OrganizationCommands.Edit(
                suspended.OrganizationId,
                "Tecnologia Suspensa",
                OrganizationStatus.Suspended),
            CancellationToken.None);

        var organizations = await sender.Send(
            new SearchOrganizationsQuery("tecnologia", OrganizationStatus.Active),
            CancellationToken.None);

        var organization = Assert.Single(organizations);
        Assert.Equal("Reus Tecnologia", organization.Name);
    }
}
