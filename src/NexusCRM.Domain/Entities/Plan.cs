using NexusCRM.Domain;

namespace NexusCRM.Domain.Organizations;

public sealed class Plan
{
    private readonly HashSet<string> _enabledFeatures;

    private Plan(
        Guid id,
        string name,
        string description,
        decimal price,
        BillingPeriod billingPeriod,
        PlanLimits limits,
        IEnumerable<string> enabledFeatures,
        bool isActive)
    {
        Id = id;
        Name = name;
        Description = description;
        Price = price;
        BillingPeriod = billingPeriod;
        Limits = limits;
        IsActive = isActive;
        _enabledFeatures = enabledFeatures
            .Select(NormalizeFeature)
            .Where(feature => feature.Length > 0)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
    }

    public Guid Id { get; }

    public string Name { get; private set; }

    public string Description { get; private set; }

    public decimal Price { get; private set; }

    public BillingPeriod BillingPeriod { get; private set; }

    public PlanLimits Limits { get; private set; }

    public bool IsActive { get; private set; }

    public IReadOnlyCollection<string> EnabledFeatures => _enabledFeatures;

    public static Plan Create(
        string name,
        string description,
        decimal price,
        BillingPeriod billingPeriod,
        PlanLimits limits,
        IEnumerable<string> enabledFeatures)
    {
        if (price < 0)
        {
            throw new DomainException("Plan price cannot be negative.");
        }

        return new Plan(
            Guid.NewGuid(),
            RequiredText(name, nameof(name)),
            description.Trim(),
            price,
            billingPeriod,
            limits,
            enabledFeatures,
            isActive: true);
    }

    public bool AllowsFeature(string feature)
    {
        return _enabledFeatures.Contains(NormalizeFeature(feature));
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Activate()
    {
        IsActive = true;
    }

    private static string RequiredText(string value, string name)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainException($"{name} is required.");
        }

        return value.Trim();
    }

    private static string NormalizeFeature(string feature)
    {
        return feature.Trim().ToLowerInvariant();
    }
}
