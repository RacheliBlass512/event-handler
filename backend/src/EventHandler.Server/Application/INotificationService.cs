using EventHandler.Domain.Entities;

namespace EventHandler.Server.Application;

/// <summary>
/// Routing brain for real-time alerts: connected (per PresenceTracker) → SignalR (Mode A);
/// not connected → IWebPushSender (Mode B). See skeleton-plan.md §4a/§4b for the flows.
/// </summary>
public interface INotificationService
{
    Task NotifyEventUpdatedAsync(Event evt, CancellationToken ct);

    Task NotifyEventAssignedAsync(Event evt, Guid technicianId, CancellationToken ct);

    /// <summary>Both the outgoing and incoming technician get notified.</summary>
    Task NotifyEventTransferredAsync(Event evt, Guid fromTechnicianId, Guid toTechnicianId, CancellationToken ct);
}
