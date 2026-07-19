using MediatR;
using NexusCRM.Application.Abstractions.Persistence;
using NexusCRM.Domain;

namespace NexusCRM.Application.Plans.DeletePlan;

public sealed record DeletePlanCommand(Guid PlanId) : IRequest<DeletePlanResult>;

public sealed class DeletePlanCommandHandler(
    IPlanRepository planRepository,
    IPlanQueries planQueries,
    IUnitOfWork unitOfWork) : IRequestHandler<DeletePlanCommand, DeletePlanResult>
{
    public async Task<DeletePlanResult> Handle(
        DeletePlanCommand command,
        CancellationToken cancellationToken)
    {
        if (!await planRepository.ExistsAsync(command.PlanId, cancellationToken).ConfigureAwait(false))
        {
            throw new DomainException("Plan was not found.");
        }

        var organizationsUsingPlan = await planQueries
            .GetOrganizationsUsingPlanAsync(command.PlanId, cancellationToken)
            .ConfigureAwait(false);

        if (organizationsUsingPlan.Count > 0)
        {
            return DeletePlanResult.InUse(organizationsUsingPlan);
        }

        await planRepository.DeleteAsync(command.PlanId, cancellationToken).ConfigureAwait(false);
        await unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return DeletePlanResult.Success;
    }
}
