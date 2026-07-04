using EventHandler.Domain.Entities;

namespace EventHandler.Domain.Abstractions;

public interface IPushSubscriptionRepository
{
    Task<IReadOnlyList<PushSubscription>> GetForUserAsync(Guid userId, CancellationToken ct);
    Task AddAsync(PushSubscription subscription, CancellationToken ct);
    Task RemoveAsync(string endpoint, CancellationToken ct);
}
