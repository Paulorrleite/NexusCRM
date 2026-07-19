using NexusCRM.Domain.Organizations;

namespace NexusCRM.Application.Plans;

public sealed record PlanListItem(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    BillingPeriod BillingPeriod,
    bool IsActive);
