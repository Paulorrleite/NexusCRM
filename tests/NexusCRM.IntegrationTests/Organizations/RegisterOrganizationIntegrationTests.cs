using MediatR;
using Microsoft.Extensions.DependencyInjection;
using NexusCRM.Application.Abstractions.Persistence;
using NexusCRM.Domain.Organizations;
using NexusCRM.IntegrationTests.Infrastructure;

namespace NexusCRM.IntegrationTests.Organizations;

[Collection(IntegrationTestCollection.Name)]
public sealed class RegisterOrganizationIntegrationTests(IntegrationTestFixture fixture)
    : IntegrationTestBase(fixture)
{
    [Fact]
    public async Task RegisterOrganization_persists_organization()
    {
        using var scope = Fixture.CreateScope();
        var sender = scope.ServiceProvider.GetRequiredService<ISender>();

        var result = await sender.Send(
            OrganizationCommands.Register("Reus Tecnologia"),
            CancellationToken.None);

        var organizationQueries = scope.ServiceProvider.GetRequiredService<IOrganizationQueries>();
        var organizations = await organizationQueries.ListAsync(
            "reus",
            "reus-tecnologia",
            OrganizationStatus.Active,
            CancellationToken.None);

        var organization = Assert.Single(organizations);
        Assert.Equal(result.OrganizationId, organization.Id);
        Assert.Equal("reus-tecnologia", result.Slug);
        Assert.Equal("Reus Tecnologia", organization.Name);
        Assert.Equal("reus-tecnologia", organization.Slug);
        Assert.Equal(OrganizationStatus.Active, organization.Status);
    }

    [Fact]
    public async Task RegisterOrganization_generates_unique_slug_when_name_repeats()
    {
        using var scope = Fixture.CreateScope();
        var sender = scope.ServiceProvider.GetRequiredService<ISender>();

        var first = await sender.Send(
            OrganizationCommands.Register("Reus Tecnologia"),
            CancellationToken.None);
        var second = await sender.Send(
            OrganizationCommands.Register("Reus Tecnologia"),
            CancellationToken.None);

        Assert.Equal("reus-tecnologia", first.Slug);
        Assert.Equal("reus-tecnologia-2", second.Slug);
    }
}
