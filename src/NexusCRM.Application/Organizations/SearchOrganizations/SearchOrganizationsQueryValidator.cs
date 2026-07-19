using FluentValidation;

namespace NexusCRM.Application.Organizations.SearchOrganizations;

public sealed class SearchOrganizationsQueryValidator : AbstractValidator<SearchOrganizationsQuery>
{
    public SearchOrganizationsQueryValidator()
    {
        RuleFor(query => query.SearchTerm)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(query => query.Status)
            .IsInEnum();
    }
}
