using MediatR;
using Microsoft.Extensions.DependencyInjection;
using NexusCRM.Application.Organizations.InactivateOrganization;
using NexusCRM.Application.Organizations.ListOrganizations;
using NexusCRM.Domain.Organizations;
using NexusCRM.IntegrationTests.Infrastructure;

namespace NexusCRM.IntegrationTests.Organizations;

[Collection(IntegrationTestCollection.Name)]
public sealed class InactivateOrganizationIntegrationTests(IntegrationTestFixture fixture)
    : IntegrationTestBase(fixture)
{
    [Fact]
    public async Task InactivateOrganization_cancels_organization()
    {
        using var scope = Fixture.CreateScope();
        var sender = scope.ServiceProvider.GetRequiredService<ISender>();

        var registered = await sender.Send(
            OrganizationCommands.Register("Reus Tecnologia"),
            CancellationToken.None);

        await sender.Send(
            new InactivateOrganizationCommand(registered.OrganizationId),
            CancellationToken.None);

        var organizations = await sender.Send(
            new ListOrganizationsQuery(null, "reus-tecnologia", OrganizationStatus.Cancelled),
            CancellationToken.None);

        var organization = Assert.Single(organizations);
        Assert.Equal(registered.OrganizationId, organization.Id);
        Assert.Equal(OrganizationStatus.Cancelled, organization.Status);
    }
}
