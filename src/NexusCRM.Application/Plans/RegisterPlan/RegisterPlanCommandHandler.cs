using MediatR;
using NexusCRM.Application.Abstractions.Persistence;
using NexusCRM.Domain.Organizations;

namespace NexusCRM.Application.Plans.RegisterPlan;

public sealed record RegisterPlanCommand(
    string Name,
    string Description,
    decimal Price,
    BillingPeriod BillingPeriod,
    int MaximumUsers,
    int MaximumContacts,
    int MaximumCompanies,
    int MaximumOpportunities,
    IReadOnlyCollection<string>? EnabledFeatures) : IRequest<RegisterPlanResult>;

public sealed class RegisterPlanCommandHandler(
    IPlanRepository planRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<RegisterPlanCommand, RegisterPlanResult>
{
    public async Task<RegisterPlanResult> Handle(
        RegisterPlanCommand command,
        CancellationToken cancellationToken)
    {
        IReadOnlyCollection<string> enabledFeatures = command.EnabledFeatures ?? [];
        var limits = new PlanLimits(
            command.MaximumUsers,
            command.MaximumContacts,
            command.MaximumCompanies,
            command.MaximumOpportunities);

        var plan = Plan.Create(
            command.Name,
            command.Description,
            command.Price,
            command.BillingPeriod,
            limits,
            enabledFeatures);

        await planRepository.AddAsync(plan, cancellationToken).ConfigureAwait(false);
        await unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return new RegisterPlanResult(plan.Id);
    }
}
