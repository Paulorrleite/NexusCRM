using NexusCRM.Domain.Organizations;

namespace NexusCRM.Domain.Tests.Entities;

public sealed class UserTests
{
    [Fact]
    public void Register_creates_active_user_with_normalized_email()
    {
        var createdAt = DateTimeOffset.UtcNow;

        var user = User.Register(" Paulo@Example.com ", "Paulo", "hashed-password", createdAt);

        Assert.NotEqual(Guid.Empty, user.Id);
        Assert.Equal("paulo@example.com", user.Email);
        Assert.Equal("Paulo", user.Name);
        Assert.Equal("hashed-password", user.PasswordHash);
        Assert.Equal(UserStatus.Active, user.Status);
        Assert.Equal(createdAt, user.CreatedAt);
    }

    [Fact]
    public void Register_rejects_invalid_email()
    {
        Assert.Throws<DomainException>(() =>
            User.Register("invalid-email", "Paulo", "hashed-password", DateTimeOffset.UtcNow));
    }

    [Fact]
    public void Rename_rejects_inactive_user()
    {
        var user = User.Register("paulo@example.com", "Paulo", "hashed-password", DateTimeOffset.UtcNow);
        user.Deactivate(DateTimeOffset.UtcNow);

        Assert.Throws<DomainException>(() => user.Rename("New name", DateTimeOffset.UtcNow));
    }
}
