using FluentValidation;

namespace NexusCRM.Application.Plans.RegisterPlan;

public sealed class RegisterPlanCommandValidator : AbstractValidator<RegisterPlanCommand>
{
    public RegisterPlanCommandValidator()
    {
        RuleFor(command => command.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(command => command.Description)
            .NotEmpty()
            .MaximumLength(1000);

        RuleFor(command => command.Price)
            .GreaterThanOrEqualTo(0);

        RuleFor(command => command.BillingPeriod)
            .IsInEnum();

        RuleFor(command => command.MaximumUsers)
            .GreaterThan(0);

        RuleFor(command => command.MaximumContacts)
            .GreaterThan(0);

        RuleFor(command => command.MaximumCompanies)
            .GreaterThan(0);

        RuleFor(command => command.MaximumOpportunities)
            .GreaterThan(0);

        RuleForEach(command => command.EnabledFeatures)
            .NotEmpty();
    }
}
