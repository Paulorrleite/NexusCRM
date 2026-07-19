using NexusCRM.Domain;

namespace NexusCRM.Domain.Organizations;

public sealed class User : Entity<Guid>
{
    private User(
        Guid id,
        string email,
        string name,
        string passwordHash,
        UserStatus status,
        DateTimeOffset createdAt,
        DateTimeOffset updatedAt)
        : base(id)
    {
        Email = email;
        Name = name;
        PasswordHash = passwordHash;
        Status = status;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public string Email { get; }

    public string Name { get; private set; }

    public string PasswordHash { get; private set; }

    public UserStatus Status { get; private set; }

    public DateTimeOffset CreatedAt { get; }

    public DateTimeOffset UpdatedAt { get; private set; }

    public bool IsActive => Status == UserStatus.Active;

    public static User Register(
        string email,
        string name,
        string passwordHash,
        DateTimeOffset createdAt)
    {
        return new User(
            Guid.NewGuid(),
            NormalizeEmail(email),
            RequiredText(name, nameof(name)),
            RequiredText(passwordHash, nameof(passwordHash)),
            UserStatus.Active,
            createdAt,
            createdAt);
    }

    public void Rename(string name, DateTimeOffset updatedAt)
    {
        EnsureActive();

        Name = RequiredText(name, nameof(name));
        UpdatedAt = updatedAt;
    }

    public void ChangePasswordHash(string passwordHash, DateTimeOffset updatedAt)
    {
        EnsureActive();

        PasswordHash = RequiredText(passwordHash, nameof(passwordHash));
        UpdatedAt = updatedAt;
    }

    public void Deactivate(DateTimeOffset updatedAt)
    {
        EnsureActive();

        Status = UserStatus.Inactive;
        UpdatedAt = updatedAt;
    }

    private void EnsureActive()
    {
        if (Status != UserStatus.Active)
        {
            throw new DomainException("The user is not active.");
        }
    }

    private static string NormalizeEmail(string email)
    {
        var normalized = RequiredText(email, nameof(email)).ToLowerInvariant();

        if (!normalized.Contains('@', StringComparison.Ordinal))
        {
            throw new DomainException("Email must be valid.");
        }

        return normalized;
    }

    private static string RequiredText(string value, string name)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainException($"{name} is required.");
        }

        return value.Trim();
    }
}
