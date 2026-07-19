using MediatR;
using NexusCRM.Application.Abstractions.Persistence;
using NexusCRM.Domain;
using NexusCRM.Domain.Organizations;

namespace NexusCRM.Application.Plans.EditPlan;

public sealed record EditPlanCommand(
    Guid PlanId,
    string Name,
    string Description,
    decimal Price,
    BillingPeriod BillingPeriod,
    int MaximumUsers,
    int MaximumContacts,
    int MaximumCompanies,
    int MaximumOpportunities,
    IReadOnlyCollection<string>? EnabledFeatures,
    bool IsActive) : IRequest<EditPlanResult>;

public sealed class EditPlanCommandHandler(
    IPlanRepository planRepository,
    IPlanQueries planQueries,
    IUnitOfWork unitOfWork) : IRequestHandler<EditPlanCommand, EditPlanResult>
{
    public async Task<EditPlanResult> Handle(
        EditPlanCommand command,
        CancellationToken cancellationToken)
    {
        IReadOnlyCollection<string> enabledFeatures = command.EnabledFeatures ?? [];

        if (!await planRepository.ExistsAsync(command.PlanId, cancellationToken).ConfigureAwait(false))
        {
            throw new DomainException("Plan was not found.");
        }

        var organizationsUsingPlan = await planQueries
            .GetOrganizationsUsingPlanAsync(command.PlanId, cancellationToken)
            .ConfigureAwait(false);

        if (organizationsUsingPlan.Count > 0)
        {
            return EditPlanResult.InUse(organizationsUsingPlan);
        }

        var limits = new PlanLimits(
            command.MaximumUsers,
            command.MaximumContacts,
            command.MaximumCompanies,
            command.MaximumOpportunities);

        _ = Plan.Create(
            command.Name,
            command.Description,
            command.Price,
            command.BillingPeriod,
            limits,
            enabledFeatures);

        await planRepository.UpdateAsync(
            command.PlanId,
            command.Name,
            command.Description,
            command.Price,
            command.BillingPeriod,
            limits,
            enabledFeatures,
            command.IsActive,
            cancellationToken).ConfigureAwait(false);

        await unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return EditPlanResult.Success;
    }
}
