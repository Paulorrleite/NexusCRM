namespace NexusCRM.Application.Plans.EditPlan;

public sealed record EditPlanResult(
    bool Updated,
    IReadOnlyCollection<PlanUsageOrganization> OrganizationsUsingPlan)
{
    public static EditPlanResult Success { get; } = new(true, []);

    public static EditPlanResult InUse(IReadOnlyCollection<PlanUsageOrganization> organizations)
    {
        return new EditPlanResult(false, organizations);
    }
}
