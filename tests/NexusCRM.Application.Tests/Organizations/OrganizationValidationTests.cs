using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using NexusCRM.Application;
using NexusCRM.Application.Organizations.EditOrganization;
using NexusCRM.Application.Organizations.InactivateOrganization;
using NexusCRM.Application.Organizations.ListOrganizations;
using NexusCRM.Application.Organizations.RegisterOrganization;
using NexusCRM.Application.Organizations.SearchOrganizations;
using NexusCRM.Domain.Organizations;

namespace NexusCRM.Application.Tests.Organizations;

public sealed class OrganizationValidationTests
{
    [Fact]
    public async Task RegisterOrganizationCommandValidator_rejects_invalid_organization_input()
    {
        var validator = new RegisterOrganizationCommandValidator();

        var result = await validator.ValidateAsync(
            new RegisterOrganizationCommand(""));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(RegisterOrganizationCommand.Name));
    }

    [Fact]
    public async Task EditOrganizationCommandValidator_rejects_empty_id_and_cancelled_status()
    {
        var validator = new EditOrganizationCommandValidator();

        var result = await validator.ValidateAsync(
            new EditOrganizationCommand(
                Guid.Empty,
                "",
                OrganizationStatus.Cancelled));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(EditOrganizationCommand.OrganizationId));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(EditOrganizationCommand.Name));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(EditOrganizationCommand.Status));
    }

    [Fact]
    public async Task InactivateOrganizationCommandValidator_rejects_empty_id()
    {
        var validator = new InactivateOrganizationCommandValidator();

        var result = await validator.ValidateAsync(new InactivateOrganizationCommand(Guid.Empty));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(InactivateOrganizationCommand.OrganizationId));
    }

    [Fact]
    public async Task ListOrganizationsQueryValidator_rejects_invalid_optional_filters()
    {
        var validator = new ListOrganizationsQueryValidator();

        var result = await validator.ValidateAsync(
            new ListOrganizationsQuery(
                new string('a', 201),
                new string('a', 201),
                (OrganizationStatus)999));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(ListOrganizationsQuery.Name));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(ListOrganizationsQuery.Slug));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(ListOrganizationsQuery.Status));
    }

    [Fact]
    public async Task SearchOrganizationsQueryValidator_rejects_empty_search_term()
    {
        var validator = new SearchOrganizationsQueryValidator();

        var result = await validator.ValidateAsync(new SearchOrganizationsQuery("", (OrganizationStatus)999));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(SearchOrganizationsQuery.SearchTerm));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(SearchOrganizationsQuery.Status));
    }

    [Fact]
    public async Task AddApplication_registers_organization_validation_pipeline()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddApplication();
        services.AddSingleton<NexusCRM.Application.Abstractions.Persistence.IOrganizationRepository, FakeOrganizationRepository>();
        services.AddSingleton<NexusCRM.Application.Abstractions.Persistence.IUnitOfWork, FakeUnitOfWork>();

        await using var serviceProvider = services.BuildServiceProvider();
        var sender = serviceProvider.GetRequiredService<MediatR.ISender>();

        await Assert.ThrowsAsync<ValidationException>(
            () => sender.Send(
                new RegisterOrganizationCommand(""),
                CancellationToken.None));
    }
}
