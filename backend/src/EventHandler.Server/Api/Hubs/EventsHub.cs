using System.Security.Claims;
using EventHandler.Server.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace EventHandler.Server.Api.Hubs;

/// <summary>
/// Connection/presence handshake is implemented for real (skeleton-plan.md §13.4 expects a
/// live SignalR connection). Pushing EventCreated/EventUpdated/EventAssigned/Alert is driven
/// externally by NotificationService via IHubContext, targeting the per-user "user:{id}" groups
/// this hub maintains — this class has no client-invocable RPC methods of its own.
/// </summary>
[Authorize]
public sealed class EventsHub : Hub
{
    private readonly IPresenceTracker _presenceTracker;

    public EventsHub(IPresenceTracker presenceTracker)
    {
        _presenceTracker = presenceTracker;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = GetUserId();
        _presenceTracker.MarkConnected(userId, Context.ConnectionId);

        await Groups.AddToGroupAsync(Context.ConnectionId, $"user:{userId}");
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _presenceTracker.MarkDisconnected(GetUserId(), Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }

    private Guid GetUserId()
    {
        var value = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(value, out var id)
            ? id
            : throw new InvalidOperationException("Hub connection missing a valid user id claim.");
    }
}
