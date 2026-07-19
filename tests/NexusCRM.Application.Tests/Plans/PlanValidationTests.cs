using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NexusCRM.Application;
using NexusCRM.Application.Plans.DeletePlan;
using NexusCRM.Application.Plans.EditPlan;
using NexusCRM.Application.Plans.ListPlans;
using NexusCRM.Application.Plans.RegisterPlan;
using NexusCRM.Domain.Organizations;

namespace NexusCRM.Application.Tests.Plans;

public sealed class PlanValidationTests
{
    [Fact]
    public async Task RegisterPlanCommandValidator_rejects_invalid_plan_input()
    {
        var validator = new RegisterPlanCommandValidator();
        var command = new RegisterPlanCommand(
            "",
            "",
            -1,
            (BillingPeriod)999,
            0,
            0,
            0,
            0,
            [""]);

        var result = await validator.ValidateAsync(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(RegisterPlanCommand.Name));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(RegisterPlanCommand.Description));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(RegisterPlanCommand.Price));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(RegisterPlanCommand.BillingPeriod));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(RegisterPlanCommand.MaximumUsers));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(RegisterPlanCommand.MaximumContacts));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(RegisterPlanCommand.MaximumCompanies));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(RegisterPlanCommand.MaximumOpportunities));
        Assert.Contains(result.Errors, error => error.PropertyName == "EnabledFeatures[0]");
    }

    [Fact]
    public async Task EditPlanCommandValidator_rejects_empty_plan_id()
    {
        var validator = new EditPlanCommandValidator();

        var result = await validator.ValidateAsync(
            new EditPlanCommand(
                Guid.Empty,
                "Professional",
                "Professional plan",
                49,
                BillingPeriod.Monthly,
                10,
                1_000,
                100,
                500,
                ["Contacts.Read"],
                IsActive: true));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(EditPlanCommand.PlanId));
    }

    [Fact]
    public async Task DeletePlanCommandValidator_rejects_empty_plan_id()
    {
        var validator = new DeletePlanCommandValidator();

        var result = await validator.ValidateAsync(new DeletePlanCommand(Guid.Empty));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(DeletePlanCommand.PlanId));
    }

    [Fact]
    public async Task ListPlansQueryValidator_rejects_invalid_optional_filters()
    {
        var validator = new ListPlansQueryValidator();
        var query = new ListPlansQuery(
            new string('a', 201),
            new string('a', 1001),
            (BillingPeriod)999,
            true);

        var result = await validator.ValidateAsync(query);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(ListPlansQuery.Name));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(ListPlansQuery.Description));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(ListPlansQuery.BillingPeriod));
    }

    [Fact]
    public async Task AddApplication_registers_validation_pipeline()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddApplication();
        services.AddSingleton<NexusCRM.Application.Abstractions.Persistence.IPlanRepository, FakePlanRepository>();
        services.AddSingleton<NexusCRM.Application.Abstractions.Persistence.IUnitOfWork, FakeUnitOfWork>();

        await using var serviceProvider = services.BuildServiceProvider();
        var sender = serviceProvider.GetRequiredService<MediatR.ISender>();

        await Assert.ThrowsAsync<ValidationException>(
            () => sender.Send(
                new RegisterPlanCommand(
                    "",
                    "Professional plan",
                    49,
                    BillingPeriod.Monthly,
                    10,
                    1_000,
                    100,
                    500,
                    ["Contacts.Read"]),
                CancellationToken.None));
    }
}
