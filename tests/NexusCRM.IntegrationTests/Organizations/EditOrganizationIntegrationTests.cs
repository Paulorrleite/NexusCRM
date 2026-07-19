using MediatR;
using Microsoft.Extensions.DependencyInjection;
using NexusCRM.Application.Organizations.ListOrganizations;
using NexusCRM.Domain.Organizations;
using NexusCRM.IntegrationTests.Infrastructure;

namespace NexusCRM.IntegrationTests.Organizations;

[Collection(IntegrationTestCollection.Name)]
public sealed class EditOrganizationIntegrationTests(IntegrationTestFixture fixture)
    : IntegrationTestBase(fixture)
{
    [Fact]
    public async Task EditOrganization_updates_name_and_status()
    {
        using var scope = Fixture.CreateScope();
        var sender = scope.ServiceProvider.GetRequiredService<ISender>();

        var registered = await sender.Send(
            OrganizationCommands.Register("Reus Tecnologia"),
            CancellationToken.None);

        await sender.Send(
            OrganizationCommands.Edit(
                registered.OrganizationId,
                "Reus Labs",
                OrganizationStatus.Suspended),
            CancellationToken.None);

        var organizations = await sender.Send(
            new ListOrganizationsQuery("labs", null, OrganizationStatus.Suspended),
            CancellationToken.None);

        var organization = Assert.Single(organizations);
        Assert.Equal(registered.OrganizationId, organization.Id);
        Assert.Equal("Reus Labs", organization.Name);
        Assert.Equal(OrganizationStatus.Suspended, organization.Status);
    }
}
