using EventHandler.Domain.Abstractions;
using EventHandler.Domain.Entities;
using EventHandler.Server.Api.Dtos;
using EventHandler.Server.Api.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace EventHandler.Server.Application;

public sealed class NotificationService : INotificationService
{
    private readonly IHubContext<EventsHub> _hub;
    private readonly IUserRepository _userRepository;
    private readonly IPresenceTracker _presenceTracker;
    private readonly IPushSubscriptionRepository _pushSubscriptions;
    private readonly IWebPushSender _webPushSender;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(
        IHubContext<EventsHub> hub,
        IUserRepository userRepository,
        IPresenceTracker presenceTracker,
        IPushSubscriptionRepository pushSubscriptions,
        IWebPushSender webPushSender,
        ILogger<NotificationService> logger)
    {
        _hub = hub;
        _userRepository = userRepository;
        _presenceTracker = presenceTracker;
        _pushSubscriptions = pushSubscriptions;
        _webPushSender = webPushSender;
        _logger = logger;
    }

    // Public contract — event-semantic, one line each. Delivery is best-effort: these never throw to
    // the caller (see INotificationService), so callers just await without a guard.
    public Task NotifyEventCreatedAsync(Event evt, CancellationToken ct)
        => NotifyDispatchersAsync("EventCreated", evt.ToDto(), ct);

    public Task NotifyEventAssignedAsync(Event evt, Guid technicianId, CancellationToken ct)
        => throw new NotImplementedException();
        // => NotifyAssigneeAsync(evt, "EventAssigned", evt.ToDto(), ct)

    public Task NotifyEventUpdatedAsync(Event evt, CancellationToken ct)
        => throw new NotImplementedException();
        // => NotifyAssigneeAsync(evt, "EventUpdated", evt.ToDto(), ct)

    public Task NotifyEventTransferredAsync(Event evt, Guid fromTechnicianId, Guid toTechnicianId, CancellationToken ct)
        => throw new NotImplementedException();
        // both specific techs => NotifyUsersAsync(new[]{ fromTechnicianId, toTechnicianId }, "EventTransferred", evt.ToDto(), ct)

    // Audience layer — the two recurring recipient sets almost every call needs.
    private async Task NotifyDispatchersAsync(string messageName, EventDto dto, CancellationToken ct)
    {
        IReadOnlyList<User> dispatchers;
        try
        {
            dispatchers = await _userRepository.ListDispatchersAsync(ct); // only throw site outside the per-recipient guard
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Notify {Message}: could not list dispatchers.", messageName);
            return;
        }

        await NotifyUsersAsync(dispatchers.Select(d => d.Id), messageName, dto, ct);
    }

    private Task NotifyAssigneeAsync(Event evt, string messageName, EventDto dto, CancellationToken ct)
        => evt.AssignedTechnicianId is Guid techId // nothing to do if the event is unassigned
            ? NotifyUsersAsync(new[] { techId }, messageName, dto, ct)
            : Task.CompletedTask;

    // By-id transport — presence-routes each recipient. Kept for ad-hoc recipient sets (e.g. transfer).
    private async Task NotifyUsersAsync(IEnumerable<Guid> userIds, string messageName, EventDto dto, CancellationToken ct)
    {
        foreach (var userId in userIds)
        {
            try
            {
                if (_presenceTracker.IsConnected(userId))
                    await _hub.Clients.Group($"user:{userId}").SendAsync(messageName, dto, ct); // Mode A
                else
                    await NotifyOfflineAsync(userId, dto, ct);                                   // Mode B
            }
            catch (Exception ex)
            {
                // Best-effort: one recipient's failure must not starve the others or fail the caller.
                _logger.LogWarning(ex, "Notify {Message} failed for {UserId}.", messageName, userId);
            }
        }
    }

    // Mode B — offline delivery via web-push. The sender + subscription repo are stubs this iteration
    // (they throw); the seam is wired so a future dev only has to fill in WebPushSender.
    private async Task NotifyOfflineAsync(Guid userId, EventDto dto, CancellationToken ct)
    {
        foreach (var sub in await _pushSubscriptions.GetForUserAsync(userId, ct))
            await _webPushSender.SendAsync(sub, dto.Title, dto.Location, $"/events/{dto.Id}", ct);
    }
}
