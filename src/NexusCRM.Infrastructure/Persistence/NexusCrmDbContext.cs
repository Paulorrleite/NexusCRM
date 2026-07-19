using Microsoft.EntityFrameworkCore;
using NexusCRM.Application.Abstractions.Persistence;
using NexusCRM.Infrastructure.Persistence.Models;

namespace NexusCRM.Infrastructure.Persistence;

public sealed class NexusCrmDbContext(DbContextOptions<NexusCrmDbContext> options)
    : DbContext(options), IUnitOfWork
{
    internal DbSet<PlanRecord> Plans => Set<PlanRecord>();

    internal DbSet<OrganizationRecord> Organizations => Set<OrganizationRecord>();

    internal DbSet<SubscriptionRecord> Subscriptions => Set<SubscriptionRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(NexusCrmDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
