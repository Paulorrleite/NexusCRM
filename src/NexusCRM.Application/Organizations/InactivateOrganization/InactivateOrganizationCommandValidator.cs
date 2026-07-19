using FluentValidation;

namespace NexusCRM.Application.Organizations.InactivateOrganization;

public sealed class InactivateOrganizationCommandValidator : AbstractValidator<InactivateOrganizationCommand>
{
    public InactivateOrganizationCommandValidator()
    {
        RuleFor(command => command.OrganizationId)
            .NotEmpty();
    }
}
