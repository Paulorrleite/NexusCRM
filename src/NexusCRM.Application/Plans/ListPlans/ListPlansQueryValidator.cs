using FluentValidation;

namespace NexusCRM.Application.Plans.ListPlans;

public sealed class ListPlansQueryValidator : AbstractValidator<ListPlansQuery>
{
    public ListPlansQueryValidator()
    {
        RuleFor(query => query.Name)
            .MaximumLength(200);

        RuleFor(query => query.Description)
            .MaximumLength(1000);

        RuleFor(query => query.BillingPeriod)
            .IsInEnum();
    }
}
