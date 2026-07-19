using FluentValidation;
using NexusCRM.Domain.Organizations;

namespace NexusCRM.Application.Organizations.EditOrganization;

public sealed class EditOrganizationCommandValidator : AbstractValidator<EditOrganizationCommand>
{
    public EditOrganizationCommandValidator()
    {
        RuleFor(command => command.OrganizationId)
            .NotEmpty();

        RuleFor(command => command.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(command => command.Status)
            .IsInEnum()
            .Must(status => status is OrganizationStatus.Active or OrganizationStatus.Suspended)
            .WithMessage("Status must be Active or Suspended.");
    }
}
