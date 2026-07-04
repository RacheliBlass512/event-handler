using EventHandler.Domain.Abstractions;
using EventHandler.Domain.Entities;

namespace EventHandler.Server.Infrastructure.Persistence;

public sealed class PushSubscriptionRepository : IPushSubscriptionRepository
{
    private readonly AppDbContext _dbContext;

    public PushSubscriptionRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<IReadOnlyList<PushSubscription>> GetForUserAsync(Guid userId, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task AddAsync(PushSubscription subscription, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task RemoveAsync(string endpoint, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
