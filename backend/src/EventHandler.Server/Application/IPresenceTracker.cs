namespace EventHandler.Server.Application;

/// <summary>Fed by EventsHub's OnConnectedAsync/OnDisconnectedAsync.</summary>
public interface IPresenceTracker
{
    void MarkConnected(Guid userId, string connectionId);
    void MarkDisconnected(Guid userId, string connectionId);
    bool IsConnected(Guid userId);
}
