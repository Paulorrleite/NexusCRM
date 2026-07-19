using FluentValidation;

namespace NexusCRM.Application.Organizations.RegisterOrganization;

public sealed class RegisterOrganizationCommandValidator : AbstractValidator<RegisterOrganizationCommand>
{
    public RegisterOrganizationCommandValidator()
    {
        RuleFor(command => command.Name)
            .NotEmpty()
            .MaximumLength(200);
    }
}
