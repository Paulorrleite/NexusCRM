using FluentValidation;

namespace NexusCRM.Application.Organizations.ListOrganizations;

public sealed class ListOrganizationsQueryValidator : AbstractValidator<ListOrganizationsQuery>
{
    public ListOrganizationsQueryValidator()
    {
        RuleFor(query => query.Name)
            .MaximumLength(200);

        RuleFor(query => query.Slug)
            .MaximumLength(200);

        RuleFor(query => query.Status)
            .IsInEnum();
    }
}
