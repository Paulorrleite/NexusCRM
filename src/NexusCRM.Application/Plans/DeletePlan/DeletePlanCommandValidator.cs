using FluentValidation;

namespace NexusCRM.Application.Plans.DeletePlan;

public sealed class DeletePlanCommandValidator : AbstractValidator<DeletePlanCommand>
{
    public DeletePlanCommandValidator()
    {
        RuleFor(command => command.PlanId)
            .NotEmpty();
    }
}
