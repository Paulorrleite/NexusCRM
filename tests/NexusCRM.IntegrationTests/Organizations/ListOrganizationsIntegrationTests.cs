using MediatR;
using Microsoft.Extensions.DependencyInjection;
using NexusCRM.Application.Organizations.ListOrganizations;
using NexusCRM.Domain.Organizations;
using NexusCRM.IntegrationTests.Infrastructure;

namespace NexusCRM.IntegrationTests.Organizations;

[Collection(IntegrationTestCollection.Name)]
public sealed class ListOrganizationsIntegrationTests(IntegrationTestFixture fixture)
    : IntegrationTestBase(fixture)
{
    [Fact]
    public async Task ListOrganizations_filters_by_name_slug_and_status()
    {
        using var scope = Fixture.CreateScope();
        var sender = scope.ServiceProvider.GetRequiredService<ISender>();

        await sender.Send(
            OrganizationCommands.Register("Reus Tecnologia"),
            CancellationToken.None);
        await sender.Send(
            OrganizationCommands.Register("Acme Sales"),
            CancellationToken.None);

        var organizations = await sender.Send(
            new ListOrganizationsQuery("reus", "tecnologia", OrganizationStatus.Active),
            CancellationToken.None);

        var organization = Assert.Single(organizations);
        Assert.Equal("Reus Tecnologia", organization.Name);
    }
}
