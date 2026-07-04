using System.Collections.Concurrent;

namespace EventHandler.Server.Application;

/// <summary>
/// In-memory presence registry: userId -> live connection ids. A user counts as "connected"
/// while they hold at least one open SignalR connection (multiple tabs/devices supported).
/// Implemented for real (unlike the rest of Application/Infrastructure) — it's pure in-memory
/// bookkeeping with no business decision to defer, and EventsHub's connection handshake
/// depends on it working (skeleton-plan.md §13.4 integration check).
/// </summary>
public sealed class PresenceTracker : IPresenceTracker
{
    private readonly ConcurrentDictionary<Guid, ConcurrentDictionary<string, byte>> _connectionsByUser = new();

    public void MarkConnected(Guid userId, string connectionId)
    {
        var connections = _connectionsByUser.GetOrAdd(userId, static _ => new ConcurrentDictionary<string, byte>());
        connections[connectionId] = 0;
    }

    public void MarkDisconnected(Guid userId, string connectionId)
    {
        if (_connectionsByUser.TryGetValue(userId, out var connections))
        {
            connections.TryRemove(connectionId, out _);
        }
    }

    public bool IsConnected(Guid userId) =>
        _connectionsByUser.TryGetValue(userId, out var connections) && !connections.IsEmpty;
}
