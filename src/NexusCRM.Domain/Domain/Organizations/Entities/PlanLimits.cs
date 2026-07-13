using NexusCRM.Domain;

namespace NexusCRM.Domain.Organizations;

public readonly record struct PlanLimits
{
    public PlanLimits(
        int maximumUsers,
        int maximumContacts,
        int maximumCompanies,
        int maximumOpportunities)
    {
        EnsurePositive(maximumUsers, nameof(MaximumUsers));
        EnsurePositive(maximumContacts, nameof(MaximumContacts));
        EnsurePositive(maximumCompanies, nameof(MaximumCompanies));
        EnsurePositive(maximumOpportunities, nameof(MaximumOpportunities));

        MaximumUsers = maximumUsers;
        MaximumContacts = maximumContacts;
        MaximumCompanies = maximumCompanies;
        MaximumOpportunities = maximumOpportunities;
    }

    public int MaximumUsers { get; }

    public int MaximumContacts { get; }

    public int MaximumCompanies { get; }

    public int MaximumOpportunities { get; }

    private static void EnsurePositive(int value, string name)
    {
        if (value <= 0)
        {
            throw new DomainException($"{name} must be greater than zero.");
        }
    }
}
