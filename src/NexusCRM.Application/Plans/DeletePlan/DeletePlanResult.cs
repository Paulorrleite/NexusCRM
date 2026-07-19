namespace NexusCRM.Application.Plans.DeletePlan;

public sealed record DeletePlanResult(
    bool Deleted,
    IReadOnlyCollection<PlanUsageOrganization> OrganizationsUsingPlan)
{
    public static DeletePlanResult Success { get; } = new(true, []);

    public static DeletePlanResult InUse(IReadOnlyCollection<PlanUsageOrganization> organizations)
    {
        return new DeletePlanResult(false, organizations);
    }
}
